using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreePassBot2.Data;

namespace TreePassBot2.BotEngine.Message.Routers.Event;

public static class WithdrawMessageFlagger
{
    public static async Task FlagMessageAsync(long msgId, ulong operatorId, IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var message = await dbContext.MessageLogs
                                     .FirstOrDefaultAsync(log => log.MessageId == msgId)
                                     .ConfigureAwait(false);

        if (message is null)
        {
            return;
        }

        message.IsRecalled = true;
        message.RecalledBy = operatorId;

        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
