using Microsoft.EntityFrameworkCore;
using TreePassBot2.Core.Entities;

namespace TreePassBot2.Data;

public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<QqUser> Users { get; set; }
    public DbSet<AuditRequestData> AuditRequests { get; set; }
    public DbSet<MessageLog> MessageLogs { get; set; }
    public DbSet<PluginState> PluginStates { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QqUser>()
                    .HasIndex(user => user.QqId)
                    .IsUnique();

        modelBuilder.Entity<AuditRequestData>()
                    .HasIndex(audit => audit.Passcode);
        modelBuilder.Entity<AuditRequestData>()
                    .HasIndex(audit => new
                    {
                        audit.RequestQqId,
                        audit.Status
                    });

        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => msg.SendAt);
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => new { msg.GroupId, msg.UserId });
        modelBuilder.Entity<MessageLog>()
                    .HasIndex(msg => msg.UserNickName)
                    .HasFilter("UserNickName IS NOT NULL");

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