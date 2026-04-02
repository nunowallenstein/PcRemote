using RemoteServer.Controllers;
using RemoteServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IInputService>(InputServiceFactory.Create());

var app = builder.Build();

app.MapControllers();

app.Run();
