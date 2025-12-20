using Microsoft.EntityFrameworkCore;
using TreePassBot2.Core.Entities;

namespace TreePassBot2.Data;

public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<GroupInfo> Groups { get; set; }
    public DbSet<QqUserInfo> Users { get; set; }
    public DbSet<AuditRequestData> AuditRequests { get; set; }
    public DbSet<MessageLog> MessageLogs { get; set; }
    public DbSet<PluginState> PluginStates { get; set; }
    public DbSet<ArchivedMessageLog> ArchivedMessageLogs { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // groups
        modelBuilder.Entity<GroupInfo>()
                    .HasIndex(group => group.GroupId)
                    .IsUnique();

        // users
        modelBuilder.Entity<QqUserInfo>()
                    .HasIndex(user => user.QqId)
                    .IsUnique();

        // audits
        modelBuilder.Entity<AuditRequestData>()
                    .HasIndex(audit => audit.VerificationCode);
        modelBuilder.Entity<AuditRequestData>()
                    .HasIndex(audit => new
                    {
                        RequestQqId = audit.UserId,
                        audit.Status
                    });

        // message logs
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => msg.SendAt);
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => msg.MessageId)
                    .IsUnique();
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => new
                    {
                        msg.GroupId,
                        msg.UserId,
                        msg.SendAt
                    });
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => msg.UserName);
        //.HasFilter("\"UserName\" IS NOT NULL");

        // archive message logs
        modelBuilder.Entity<ArchivedMessageLog>()
                    .HasIndex(msg => new { msg.GroupId, msg.UserId });
        modelBuilder.Entity<ArchivedMessageLog>()
                    .HasIndex(msg => msg.UserNickName);
        //.HasFilter("\"UserNickName\" IS NOT NULL");

        // plugin states
        modelBuilder.Entity<PluginState>()
                    .HasKey(plg => new
                    {
                        plg.PluginId,
                        Scoop = plg.Scope,
                        plg.GroupId,
                        plg.UserId
                    });
    }
}
