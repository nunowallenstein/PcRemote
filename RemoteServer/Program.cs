using System.Net;
using RemoteServer.Controllers;
using RemoteServer.Services;
using RemoteServer.Services.Windows.KeyboardInput;

namespace RemoteServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            if (OperatingSystem.IsWindows())
            {
                builder.Services.AddSingleton<IKeyPressService, KeyPressService>();
                builder.Services.AddSingleton<IWritingService, WritingService>();
                builder.Services.AddSingleton<IMouseInput, RemoteServer.Services.Windows.MouseInputService>();
                builder.Services.AddSingleton<IMediaInput, RemoteServer.Services.Windows.MediaInputService>();
            }
            else if (OperatingSystem.IsLinux())
            {
                builder.Services.AddSingleton<RemoteServer.Services.Linux.IKeyboardInput, RemoteServer.Services.Linux.KeyboardInputService>();
                builder.Services.AddSingleton<IMouseInput, RemoteServer.Services.Linux.MouseInputService>();
                builder.Services.AddSingleton<IMediaInput, RemoteServer.Services.Linux.MediaInputService>();
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
