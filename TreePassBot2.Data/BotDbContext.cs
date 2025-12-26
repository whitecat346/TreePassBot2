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
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ulong>()
                            .HaveConversion<long>();
        configurationBuilder.Properties<ulong?>()
                            .HaveConversion<long?>();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // groups
        modelBuilder.Entity<GroupInfo>(entity =>
        {
            entity.HasKey(g => g.GroupId);
            entity.Property(g => g.Name).HasMaxLength(200);
        });
        modelBuilder.Entity<GroupInfo>()
                    .ToTable(t => t.HasCheckConstraint("CK_Group_Id_Positive", "\"GroupId\" > 0"));

        // users
        modelBuilder.Entity<QqUserInfo>(entity =>
        {
            entity.HasIndex(user => new { user.QqId, user.GroupId }).IsUnique();

            entity.HasOne(user => user.Group)
                  .WithMany(group => group.Members)
                  .HasForeignKey(user => user.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // audits
        modelBuilder.Entity<AuditRequestData>(entity =>
        {
            entity.HasOne(a => a.Group)
                  .WithMany(g => g.AuditRequests)
                  .HasForeignKey(a => a.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // message logs
        modelBuilder.Entity<MessageLog>(entity =>
        {
            entity.HasOne(m => m.Group)
                  .WithMany(g => g.Messages)
                  .HasForeignKey(m => m.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => m.SendAt).HasMethod("brin");
            entity.HasIndex(m => new { m.GroupId, m.MessageId });
            entity.HasIndex(m => new { m.GroupId, m.SendAt });
        });

        // archive message logs
        modelBuilder.Entity<ArchivedMessageLog>()
                    .HasIndex(msg => msg.SendAt)
                    .HasMethod("brin");
        modelBuilder.Entity<ArchivedMessageLog>()
                    .HasIndex(msg => new { msg.GroupId, msg.MessageId })
                    .IncludeProperties(msg => new { msg.UserId, msg.SendAt });
        modelBuilder.Entity<ArchivedMessageLog>()
                    .HasIndex(msg => msg.Content)
                    .HasMethod("gin")
                    .HasOperators("gin_trgm_ops");

        // plugin states
        modelBuilder.Entity<PluginState>()
                    .HasKey(plg => new
                    {
                        plg.PluginId,
                        Scoop = plg.Scope,
                        plg.GroupId,
                        plg.UserId
                    });
        modelBuilder.Entity<PluginState>()
                    .HasIndex(p => p.StateDataJson)
                    .HasMethod("gin");
    }
}
