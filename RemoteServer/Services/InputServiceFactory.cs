using System.Runtime.InteropServices;

namespace RemoteServer.Services;

public static class InputServiceFactory
{
    public static IInputService Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new WindowsInputService();
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new LinuxInputService();

        throw new NotSupportedException("Operating system not supported");
    }
}
