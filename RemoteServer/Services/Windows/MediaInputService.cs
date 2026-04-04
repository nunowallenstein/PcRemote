using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace RemoteServer.Services.Windows;

public class MediaInputService : IMediaInput
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

    public void SetVolume(int level)
    {
        try
        {
            level = Math.Clamp(level, 0, 100);
            
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level / 100f;
            
            Console.WriteLine($"[WindowsMedia] SetVolume: {level}%");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WindowsMedia] SetVolume error: {ex.Message}");
        }
    }

    private void SendMediaKey(string name)
    {
        var vk = GetMediaVk(name);
        if (vk != 0)
        {
            Console.WriteLine($"[WindowsMedia] {name} (vk=0x{vk:X2})");
            keybd_event(vk, 0, 0, 0);
            Console.WriteLine($"[WindowsMedia] {name} sent");
        }
    }

    private static byte GetMediaVk(string name)
    {
        return name.ToLower() switch
        {
            "next" => 0xB0,
            "prev" => 0xB1,
            "play" => 0xB3,
            "pause" => 0xB3,
            "stop" => 0xB2,
            "mute" => 0xAD,
            "volup" => 0xAF,
            "voldown" => 0xAE,
            _ => (byte)0
        };
    }
}
