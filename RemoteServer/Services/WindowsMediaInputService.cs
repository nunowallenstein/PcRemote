using System.Runtime.InteropServices;

namespace RemoteServer.Services;

public class WindowsMediaInputService : IMediaInput
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    public void Next() => SendMediaKey("next");
    public void Previous() => SendMediaKey("prev");
    public void PlayPause() => SendMediaKey("play");
    public void Stop() => SendMediaKey("stop");
    public void Mute() => SendMediaKey("mute");
    public void VolumeUp() => SendMediaKey("volup");
    public void VolumeDown() => SendMediaKey("voldown");

    private void SendMediaKey(string name)
    {
        var vk = WindowsKeyMapper.GetMediaVk(name);
        if (vk != 0)
        {
            Console.WriteLine($"[WindowsMedia] {name} (vk=0x{vk:X2})");
            keybd_event(vk, 0, 0, 0);
            Console.WriteLine($"[WindowsMedia] {name} sent");
        }
    }
}