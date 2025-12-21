using BackManager.Server.Extensions;
using Serilog;
using TreePassBot2.Infrastructure.Logger;
using TreePassBot2.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// database logging configuration
builder.ConfigureDatabaseLogger();

builder.AddServiceDefaults();

// Add services to the container.
// bot own services
builder.Services.AddBotServices(builder.Configuration);

// 添加CORS服务配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

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

// 添加CORS中间件
app.UseCors("AllowAllOrigins");

// 添加全局异常处理中间件
app.UseMiddleware<BackManager.Server.Middleware.GlobalExceptionHandler>();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
