using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TreePassBot2.Data;

namespace TreePassBot2.BotEngine.Message.Routers.Event;

public partial class WithdrawMessageFlagger(BotDbContext dbContext, ILogger<WithdrawMessageFlagger> logger)
{
    public async Task FlagMessageAsync(long msgId, ulong operatorId)
    {
        var message = await dbContext.MessageLogs
                                     .FirstOrDefaultAsync(log => log.MessageId == msgId)
                                     .ConfigureAwait(false);

        if (message is null)
        {
            return;
        }

        LogFlageMessageMsgid(logger, msgId);

        message.IsRecalled = true;
        message.RecalledBy = operatorId;

        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Trace, "Flage message: {msgId}")]
    static partial void LogFlageMessageMsgid(ILogger<WithdrawMessageFlagger> logger, long msgId);
}
