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
                           .AddJsonFile("appsettings.json", optional: true)
                           .AddJsonFile("appsettings.Development.json", optional: true)
                           .AddEnvironmentVariables()
                           .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
         ?? configuration["ConnectionStrings__Default"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Cannot found connection string 'DefaultConnection' in Design mode.");
        }

        var builder = new DbContextOptionsBuilder<BotDbContext>();
        builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("TreePassBot2.Data"));

        return new BotDbContext(builder.Options);
    }
}
