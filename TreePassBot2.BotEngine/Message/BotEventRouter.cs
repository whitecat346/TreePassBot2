using Makabaka.Events;
using Makabaka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Message.Routers.Event;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Core.Options;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Message;

public partial class BotEventRouter
{
    private readonly ILogger<BotEventRouter> _logger;
    private readonly MessageRouter _router;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotOptions _config;

    public BotEventRouter(
        IServiceProvider serviceProvider,
        IOptions<BotOptions> config)
    {
        _serviceProvider = serviceProvider;

        _logger = _serviceProvider.GetRequiredService<ILogger<BotEventRouter>>();
        _router = _serviceProvider.GetRequiredService<MessageRouter>();
        _config = config.Value;
    }

    public void StartRoute()
    {
        var makabaka = _serviceProvider.GetRequiredService<ICommunicationService>();

        makabaka.BotContext.OnGroupMessage += BotContextOnOnGroupMessageAsync;

        makabaka.BotContext.OnGroupMemberIncrease += BotContextOnOnGroupMemberIncreaseAsync;
        makabaka.BotContext.OnGroupMemberDecrease += BotContextOnOnGroupMemberDecreaseAsync;

        makabaka.BotContext.OnGroupAddRequest += BotContextOnOnGroupAddRequestAsync;

        makabaka.BotContext.OnGroupMemberMute += BotContextOnOnGroupMemberMuteAsync;

        makabaka.BotContext.OnGroupMessageWithdraw += BotContextOnOnGroupMessageWithdrawAsync;
    }

    private Task BotContextOnOnGroupMessageWithdrawAsync(object _, GroupMessageWithdrawEventArgs e)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var flagger = scope.ServiceProvider.GetRequiredService<WithdrawMessageFlagger>();

        return flagger.FlagMessageAsync(e.MessageId, e.OperatorId);
    }

    private async Task BotContextOnOnGroupMemberMuteAsync(object _, GroupMemberMuteEventArgs e)
    {
        if (e.Duration == 0)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var archiver = scope.ServiceProvider.GetRequiredService<MessageArchiveService>();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var latestMessageId = await db.ArchivedMessageLogs
                                      .Where(x => x.GroupId == e.GroupId && x.UserId == e.UserId)
                                      .OrderByDescending(x => x.Id)
                                      .Select(x => x.MessageId)
                                      .FirstOrDefaultAsync()
                                      .ConfigureAwait(false);

        if (latestMessageId == 0)
        {
            LogFialedToGetLatestMessageLog();
            return;
        }

        await archiver.ArchiveUserMessageAsync(
            e.GroupId,
            e.OperatorId,
            latestMessageId,
            $"Muted for {e.Duration}s", TimeSpan.FromHours(2)).ConfigureAwait(false);
    }

    private async Task BotContextOnOnGroupAddRequestAsync(object _, GroupAddRequestEventArgs e)
    {
        if (e.GroupId == _config.AuditGroupId)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var auditManager = scope.ServiceProvider.GetRequiredService<AuditManagerService>();

        var comment = e.Comment.Trim([' ', '\n']);
        var (isAllowed, msg) = await auditManager.CheckVerificationCodeAsync(e.UserId, comment)
                                                 .ConfigureAwait(false);

        if (isAllowed)
        {
            await e.AcceptAsync().ConfigureAwait(false);
            await auditManager.MarkJonedAsync(e.UserId).ConfigureAwait(false);
        }
        else
        {
            await e.RejectAsync(msg).ConfigureAwait(false);
        }

        LogProcessedUserJoinRequest(e.UserId, comment);
    }

    private async Task BotContextOnOnGroupMemberDecreaseAsync(object _, GroupMemberDecreaseEventArgs e)
    {
        if (e.GroupId == _config.AuditGroupId)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var archiver = scope.ServiceProvider.GetRequiredService<MessageArchiveService>();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var latestMessageId = await db.ArchivedMessageLogs
                                      .Where(x => x.GroupId == e.GroupId && x.UserId == e.UserId)
                                      .OrderByDescending(x => x.Id)
                                      .Select(x => x.MessageId)
                                      .FirstOrDefaultAsync()
                                      .ConfigureAwait(false);

        if (latestMessageId == 0)
        {
            LogFialedToGetLatestMessageLog();
            return;
        }

        var reason = e.SubType == GroupMemberDecreaseEventType.Kick ? "Kicked" : "Left";

        await archiver.ArchiveUserMessageAsync(
            e.GroupId,
            e.OperatorId,
            latestMessageId,
            reason, TimeSpan.FromHours(2)).ConfigureAwait(false);
    }

    private async Task BotContextOnOnGroupMemberIncreaseAsync(object _, GroupMemberIncreaseEventArgs e)
    {
        if (e.GroupId != _config.AuditGroupId)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var auditManager = scope.ServiceProvider.GetRequiredService<AuditManagerService>();

        await auditManager.AddAuditRequestAsync(e.UserId, e.GroupId).ConfigureAwait(false);
    }

    private Task BotContextOnOnGroupMessageAsync(object _, GroupMessageEventArgs e)
    {
        // build message event data
        var senderNickName = e.Sender?.Nickname ?? string.Empty;
        var senderData = new SenderData(e.UserId, senderNickName, e.Anonymous is null, UserRoleMapper(e.Sender?.Role));
        var msg = MessageConverter.ConvertToBotMessage(e.Message);

        var msgData = new MessageEventData(senderData, msg, e.MessageId, e.GroupId);

        LogHandleMessageMsgid(msgData.MessageId);

        return _router.ExecuteMessageHandlerAsync(msgData);
    }

    private static UserRole UserRoleMapper(GroupRoleType? role)
    {
        if (role == null)
        {
            return UserRole.Member;
        }

        return role switch
        {
            GroupRoleType.Owner => UserRole.Owner,
            GroupRoleType.Admin => UserRole.Admin,
            _ => UserRole.Member,
        };
    }

    [LoggerMessage(LogLevel.Error, "Fialed to get latest message log id")]
    partial void LogFialedToGetLatestMessageLog();

    [LoggerMessage(LogLevel.Information, "Handle message: {msgId}")]
    partial void LogHandleMessageMsgid(long msgId);

    [LoggerMessage(LogLevel.Information, "Processed user {userId} with {comment}")]
    partial void LogProcessedUserJoinRequest(ulong userId, string comment);
}
