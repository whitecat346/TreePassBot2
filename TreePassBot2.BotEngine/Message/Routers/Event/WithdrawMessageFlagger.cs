using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TreePassBot2.Data;

namespace TreePassBot2.BotEngine.Message.Routers.Event;

public partial class WithdrawMessageFlagger(BotDbContext dbContext, ILogger<WithdrawMessageFlagger> logger)
{
    public async Task FlagMessageAsync(long msgId, ulong operatorId)
    {
        try
        {
            var message = await dbContext.MessageLogs.SingleOrDefaultAsync(log => log.MessageId == msgId)
                                         .ConfigureAwait(false);
            if (message is null)
            {
                return;
            }

            LogFlageMessage(logger, msgId);

            message.IsRecalled = true;
            message.RecalledBy = operatorId;

            dbContext.Update(message);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogFailedToGetMessageFromDb(logger, ex, msgId);
        }
    }

    [LoggerMessage(LogLevel.Trace, "Flage message: {msgId}")]
    static partial void LogFlageMessage(ILogger<WithdrawMessageFlagger> logger, long msgId);

    [LoggerMessage(LogLevel.Error, "Failed to get message id in database: {id}")]
    static partial void LogFailedToGetMessageFromDb(
        ILogger<WithdrawMessageFlagger> logger, Exception ex, long id);
}
