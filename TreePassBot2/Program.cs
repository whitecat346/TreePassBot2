using Serilog;
using TreePassBot2.Extensions;
using TreePassBot2.Infrastructure.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSmartLogger();
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

// bot own services
builder.Services.AddBotServices(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
   .WithStaticAssets();


app.Run();