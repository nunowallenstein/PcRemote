using System.Net;
using RemoteServer.Services;

using WindowsServices=RemoteServer.Services.Windows;

using LinuxServices=RemoteServer.Services.Linux;

namespace RemoteServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            if (OperatingSystem.IsWindows())
            { 
                builder.Services.AddSingleton<IKeyPressService, WindowsServices.KeyboardInput.KeyPressService>();
                builder.Services.AddSingleton<IWritingService, WindowsServices.KeyboardInput.WritingService>();
                builder.Services.AddSingleton<IMouseInput,WindowsServices.MouseInputService>();
                builder.Services.AddSingleton<IMediaInput,WindowsServices.MediaInputService>();
            }
            else if (OperatingSystem.IsLinux())
            {
                builder.Services.AddSingleton<LinuxServices.IKeyboardInput,LinuxServices.KeyboardInputService>();
                builder.Services.AddSingleton<IMouseInput,LinuxServices.MouseInputService>();
                builder.Services.AddSingleton<IMediaInput, LinuxServices.MediaInputService>();
            }
            else
            {
                throw new PlatformNotSupportedException("Only Windows and Linux are supported.");
            }

            var app = builder.Build();

            app.Urls.Add("http://0.0.0.0:5260");

            var hostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(hostName);
            var localIps = hostEntry.AddressList
                .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Where(ip => !ip.ToString().StartsWith("127."))
                .ToList();

            Console.WriteLine("\n========================================");
            Console.WriteLine("PC Remote Server");
            Console.WriteLine("========================================");
            Console.WriteLine("Access from your phone at:");

            foreach (var ip in localIps)
            {
                Console.WriteLine($"  http://{ip}:5260");
            }

            if (localIps.Count == 0)
            {
                Console.WriteLine("  (No local IP found - check network connection)");
            }

            Console.WriteLine("Make sure your phone is on the same WiFi network.");
            Console.WriteLine("========================================\n");

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
