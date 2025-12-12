using Makabaka.Events;
using Makabaka.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Message.Routers.Event;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Message;

public class BotEventRouter
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
        var makabaka = _serviceProvider.GetRequiredService<ICommunicationService>();
        _config = config.Value;

        makabaka.BotContext.OnGroupMessage += BotContextOnOnGroupMessageAsync;

        makabaka.BotContext.OnGroupMemberIncrease += BotContextOnOnGroupMemberIncreaseAsync;
        makabaka.BotContext.OnGroupMemberDecrease += BotContextOnOnGroupMemberDecreaseAsync;

        makabaka.BotContext.OnGroupAddRequest += BotContextOnOnGroupAddRequestAsync;

        makabaka.BotContext.OnGroupMemberMute += BotContextOnOnGroupMemberMuteAsync;

        makabaka.BotContext.OnGroupMessageWithdraw += BotContextOnOnGroupMessageWithdrawAsync;
    }

    private Task BotContextOnOnGroupMessageWithdrawAsync(object _, GroupMessageWithdrawEventArgs e)
    {
        return WithdrawMessageFlagger.FlagMessageAsync(e.MessageId, e.OperatorId, _serviceProvider);
    }

    private async Task BotContextOnOnGroupMemberMuteAsync(object _, GroupMemberMuteEventArgs e)
    {
        if (e.Duration == 0)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var archiver = scope.ServiceProvider.GetRequiredService<MessageArchiveService>();

        await archiver.ArchiveUserMessageAsync(
            e.GroupId,
            e.OperatorId,
            e.UserId,
            $"Muted for {e.Duration}s", TimeSpan.FromHours(2)).ConfigureAwait(false);
    }

    private Task BotContextOnOnGroupAddRequestAsync(object _, GroupAddRequestEventArgs e)
    {
        // TODO：impl this

        return Task.CompletedTask;
    }

    private async Task BotContextOnOnGroupMemberDecreaseAsync(object _, GroupMemberDecreaseEventArgs e)
    {
        if (e.GroupId == _config.AuditGroupId)
        {
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var archiver = scope.ServiceProvider.GetRequiredService<MessageArchiveService>();

        var reason = e.SubType == GroupMemberDecreaseEventType.Kick ? "Kicked" : "Left";

        await archiver.ArchiveUserMessageAsync(
            e.GroupId,
            e.OperatorId,
            e.UserId,
            reason, TimeSpan.FromHours(2)).ConfigureAwait(false);
    }

    private Task BotContextOnOnGroupMemberIncreaseAsync(object _, GroupMemberIncreaseEventArgs e)
    {
        if (e.GroupId != _config.AuditGroupId)
        {
            return Task.CompletedTask;
        }

        // TODO: imlp this

        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupMessageAsync(object _, GroupMessageEventArgs e)
    {
        // build message event data
        var senderNickName = e.Sender?.Nickname ?? string.Empty;
        var senderData = new SenderData(e.UserId, senderNickName, e.Anonymous is null, UserRoleMapper(e.Sender?.Role));
        var msg = MessageConverter.ConvertToBotMessage(e.Message);

        var msgData = new MessageEventData(senderData, msg, e.MessageId, e.GroupId);

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
}
