using Serilog;
using TreePassBot2.Extensions;
using TreePassBot2.Infrastructure.Logger;
using TreePassBot2.ServiceDefaults;

namespace BackManager.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // database logging configuration
        builder.ConfigureSmartLogger();

        builder.AddServiceDefaults();

        // Add services to the container.
        // bot own services
        builder.Services.AddBotServices(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseHsts();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
