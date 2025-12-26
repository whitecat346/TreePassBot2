using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public partial class MessageTracer(
    BotDbContext db,
    ILogger<MessageTracer> logger) : IMessageHandler
{
    /// <inheritdoc />
    public async Task HandleMessageAsync(MessageEventData data)
    {
        var msgLog = new MessageLog
        {
            MessageId = data.MessageId,
            GroupId = data.GroupId,
            UserId = data.Sender.Id,
            UserName = data.Sender.NickName,
            Content = data.Message.ToString(),
            IsRecalled = false,
            RecalledBy = 0,
            RecalledAt = null
        };

        db.MessageLogs.Add(msgLog);
        await db.SaveChangesAsync().ConfigureAwait(false);

        LogLogMessageMsgid(logger, data.MessageId);
    }

    [LoggerMessage(LogLevel.Trace, "Log message: {MsgId}")]
    static partial void LogLogMessageMsgid(ILogger<MessageTracer> logger, long MsgId);
}
