using RemoteServer.Controllers;
using RemoteServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IKeyboardInput, WindowsKeyboardInputService>();
    builder.Services.AddSingleton<IMouseInput, WindowsMouseInputService>();
    builder.Services.AddSingleton<IMediaInput, WindowsMediaInputService>();
}
else if (OperatingSystem.IsLinux())
{
    builder.Services.AddSingleton<IKeyboardInput, LinuxKeyboardInputService>();
    builder.Services.AddSingleton<IMouseInput, LinuxMouseInputService>();
    builder.Services.AddSingleton<IMediaInput, LinuxMediaInputService>();
}
else
{
    throw new PlatformNotSupportedException("Only Windows and Linux are supported.");
}

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();