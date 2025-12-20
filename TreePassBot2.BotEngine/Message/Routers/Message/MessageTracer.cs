using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public partial class MessageTracer(
    BotDbContext db,
    ICommunicationService communication,
    ILogger<MessageTracer> logger) : IMessageHandler
{
    /// <inheritdoc />
    public async Task HandleMessageAsync(MessageEventData data)
    {
        string groupName;

        var groupInfo = await db.Groups.FindAsync(data.GroupId).ConfigureAwait(false);
        if (groupInfo is null)
        {
            var featchedGroupInfo = await communication.GetGroupInfoAsync(data.GroupId).ConfigureAwait(false);
            groupName = featchedGroupInfo is null ? string.Empty : featchedGroupInfo.Name;
        }
        else
        {
            groupName = groupInfo.Name;
        }

        var msgLog = new MessageLog
        {
            MessageId = data.MessageId,
            GroupId = data.GroupId,
            GroupName = groupName,
            UserId = data.Sender.Id,
            UserName = string.IsNullOrEmpty(data.Sender.NickName) ? null : data.Sender.NickName,
            Content = data.Message.ToString(),
            IsRecalled = false,
            RecalledBy = null,
            RecalledAt = null
        };

        LogLogMessageMsgid(logger, data.MessageId);

        db.MessageLogs.Add(msgLog);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Trace, "Log message: {MsgId}")]
    static partial void LogLogMessageMsgid(ILogger<MessageTracer> logger, long MsgId);
}
