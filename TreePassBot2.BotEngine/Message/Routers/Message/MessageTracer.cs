using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public class MessageTracer(BotDbContext db) : IMessageHandler
{
    /// <inheritdoc />
    public Task HandleMessageAsync(MessageEventData data)
    {
        var msgLog = new MessageLog
        {
            MessageId = data.MessageId,
            GroupId = data.GroupId,
            UserId = data.Sender.Id,
            UserNickName = string.IsNullOrEmpty(data.Sender.NickName) ? null : data.Sender.NickName,
            ContentText = data.Message.ToString(),
            IsWithdrawed = false,
            WithdrawedBy = null
        };

        db.MessageLogs.Add(msgLog);
        return db.SaveChangesAsync();
    }
}
