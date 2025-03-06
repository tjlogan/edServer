using System.IO.Abstractions;
using EdServer.Adapters;
using EdServer.Interfaces;
using EdServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllersWithViews();
services.AddScoped<IWebSocketManager, WebSocketManagerAdapter>();
services.AddScoped<IFileSystem, FileSystem>();
services.AddScoped<IFileSystemWatcherFactory, FileSystemWatcherFactory>();
services.AddScoped<IJournalMonitorService, JournalMonitorService>();
services.AddScoped<IFileLocationService, FileLocationService>();

var app = builder.Build();

app.UseWebSockets();
app.UseFileServer();
app.UseRouting();
app.MapControllers();

app.Run();
