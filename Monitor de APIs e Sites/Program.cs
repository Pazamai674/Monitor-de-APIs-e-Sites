using MonitorApp.Hubs;
using MonitorApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<HealthCheckWorker>();

var app = builder.Build();

app.UseStaticFiles(); 
app.UseRouting();

app.MapControllers();
app.MapHub<MonitorHub>("/monitorHub"); 

app.Run();