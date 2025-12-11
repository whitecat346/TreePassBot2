using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;

namespace TreePassBot2.BotEngine.Services;

public class MessageArchiveService(
    IServiceProvider serviceProvider,
    ILogger<MessageArchiveService> logger)
{
    public async Task ArchiveUserMessageAsync(ulong groupId, ulong userId, ulong operatorId, string reason,
                                              TimeSpan lookBackTime)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var since = DateTimeOffset.UtcNow - lookBackTime;

        var recentLogs = await db.MessageLogs
                                 .AsNoTracking()
                                 .Where(msg => msg.GroupId == groupId &&
                                               msg.UserId == userId &&
                                               msg.SendAt >= since)
                                 .ToListAsync().ConfigureAwait(false);

        if (recentLogs.Count == 0)
        {
            return;
        }

        var archives = recentLogs.Select(log => new ArchivedMessageLog
        {
            MessageId = log.MessageId,
            GroupId = log.GroupId,
            UserId = log.UserId,
            UserNickName = log.UserNickName,
            ContentText = log.ContentText,
            IsWithdrawed = log.IsWithdrawed,
            SendAt = log.SendAt,
            ArchiveReason = reason,
            OperatorId = operatorId,
            ArchivedAt = DateTimeOffset.UtcNow
        });

        await db.ArchivedMessageLogs.AddRangeAsync(archives).ConfigureAwait(false);

        await db.SaveChangesAsync().ConfigureAwait(false);

        logger.LogInformation("Archived {Count} messages for user {User} in group {Group}.\t\n Reason: {Reason}",
                              recentLogs.Count, userId, groupId, reason);
    }
}
