using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TreePassBot2.Data;

public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    /// <inheritdoc />
    public BotDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../BackManager/BackManager.Server");
        var configuration = new ConfigurationBuilder()
                           .SetBasePath(basePath)
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                           .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                           .Build();

        var connectionStr = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionStr))
        {
            throw new InvalidOperationException("Cannot found connection string 'DefaultConnection' in Design mode.");
        }

        var builder = new DbContextOptionsBuilder<BotDbContext>();
        builder.UseNpgsql(connectionStr, b => b.MigrationsAssembly("TreePassBot2.Data"));

        return new BotDbContext(builder.Options);
    }
}