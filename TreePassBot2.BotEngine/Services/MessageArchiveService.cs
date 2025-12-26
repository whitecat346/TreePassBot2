using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;

namespace TreePassBot2.BotEngine.Services;

public partial class MessageArchiveService(
    IServiceProvider serviceProvider,
    ILogger<MessageArchiveService> logger)
{
    public async Task LogMessageAsync(ulong groupId, ulong userId, long messageId, string content)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var msgLog = new MessageLog
        {
            MessageId = messageId,
            GroupId = groupId,
            UserId = userId,
            Content = content,
            SendAt = DateTimeOffset.UtcNow,
            IsRecalled = false,
            RecalledBy = 0
        };

        db.MessageLogs.Add(msgLog);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task ArchiveUserMessageAsync(ulong groupId,
                                              ulong operatorId,
                                              long startMessageId,
                                              string reason,
                                              TimeSpan? lookBackTime = null)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var anchorMsg = await db.MessageLogs
                                .Include(m => m.Group)
                                .FirstOrDefaultAsync(m => m.GroupId == groupId &&
                                                          m.MessageId == startMessageId)
                                .ConfigureAwait(false);

        if (anchorMsg == null)
        {
            return;
        }

        lookBackTime ??= TimeSpan.FromHours(1);

        var startTime = anchorMsg.SendAt - lookBackTime;
        var messagesToArchive = await db.MessageLogs
                                        .AsNoTracking()
                                        .Where(m => m.SendAt >= startTime
                                                 && m.SendAt <= anchorMsg.SendAt)
                                        .ToListAsync().ConfigureAwait(false);

        var archives = messagesToArchive.Select(m => new ArchivedMessageLog
        {
            MessageId = m.MessageId,
            GroupId = m.GroupId,
            UserId = m.UserId,
            GroupNameSnapshot = m.Group.Name,
            UserNameSnapshot = m.UserName,
            Content = m.Content,
            SendAt = m.SendAt,
            IsRecalled = m.IsRecalled,
            RecalledBy = m.RecalledBy,
            ArchiveReason = reason,
            OperatorId = operatorId
        });

        await db.ArchivedMessageLogs.AddRangeAsync(archives).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);

        LogArchiveAction(logger, messagesToArchive.Count, startMessageId, groupId, reason);
    }

    [LoggerMessage(LogLevel.Information,
                   "Archived {count} messages before {messageId} in group {group}.\t\n Reason: {reason}")]
    static partial void LogArchiveAction(
        ILogger<MessageArchiveService> logger, int count, long messageId, ulong group, string reason);
}
