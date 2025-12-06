using Makabaka.Events;
using Makabaka.Models;
using Microsoft.Extensions.Logging;
using TreePassBot2.Core.Entities;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.Services;

namespace TreePassBot2.BotEngine.Message;

public class BotEventRouter
{
    private readonly ILogger<BotEventRouter> _logger;
    private readonly MessageRouter _router;

    private BotEventRouter(
        ILogger<BotEventRouter> logger,
        MakabakaService makabaka,
        MessageRouter router)
    {
        _logger = logger;
        _router = router;

        makabaka.BotContext.OnGroupMessage += BotContextOnOnGroupMessageAsync;

        makabaka.BotContext.OnGroupMemberIncrease += BotContextOnOnGroupMemberIncreaseAsync;
        makabaka.BotContext.OnGroupMemberDecrease += BotContextOnOnGroupMemberDecreaseAsync;

        makabaka.BotContext.OnGroupAddRequest += BotContextOnOnGroupAddRequestAsync;

        makabaka.BotContext.OnGroupMemberMute += BotContextOnOnGroupMemberMuteAsync;

        makabaka.BotContext.OnGroupMessageWithdraw += BotContextOnOnGroupMessageWithdrawAsync;
    }

    private Task BotContextOnOnGroupMessageWithdrawAsync(object _, GroupMessageWithdrawEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupMemberMuteAsync(object _, GroupMemberMuteEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupAddRequestAsync(object _, GroupAddRequestEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupMemberDecreaseAsync(object _, GroupMemberDecreaseEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupMemberIncreaseAsync(object _, GroupMemberIncreaseEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task BotContextOnOnGroupMessageAsync(object _, GroupMessageEventArgs e)
    {
        // build message event data
        var senderNickName = e.Sender?.Nickname ?? string.Empty;
        var senderData = new SenderData(e.UserId, senderNickName, e.Anonymous is null, UserRoleMapper(e.Sender?.Role));
        var msg = MessageConverter.ConvertToTreePassBotMessage(e.Message);

        var msgData = new MessageEventData(senderData, msg, e.MessageId, e.GroupId);

        return _router.ExecuteMessageHandlerAsync(msgData);
    }

    private static UserRole UserRoleMapper(GroupRoleType? role)
    {
        if (role == null)
        {
            return UserRole.Member;
        }

        switch (role)
        {
            case GroupRoleType.Owner:
                return UserRole.Owner;
            case GroupRoleType.Admin:
                return UserRole.Admin;
            default:
                return UserRole.Member;
        }
    }
}
