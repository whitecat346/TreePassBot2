using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace TreePassBot2.Infrastructure.Logger;

public static class SerilogExtensions
{
    /// <summary>
    /// Add database logging using Serilog.
    /// </summary>
    public static void ConfigureSmartLogger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((_, configuration) =>
        {
            configuration.ReadFrom.Configuration(builder.Configuration);

            var connectionString = builder.Configuration.GetConnectionString("SerilogDb");

            if (!string.IsNullOrEmpty(connectionString))
            {
                var tableName = "system_logs";

                configuration.WriteTo.Async(dis =>
                                                dis.PostgreSQL(
                                                    connectionString: connectionString,
                                                    tableName: tableName,
                                                    columnOptions: LogColumnMap.GetColumnOptions(),
                                                    needAutoCreateTable: true,
                                                    restrictedToMinimumLevel: LogEventLevel.Warning));
            }

            configuration.Enrich.FromLogContext();

            configuration.Enrich.WithMachineName();
        });
    }
}
