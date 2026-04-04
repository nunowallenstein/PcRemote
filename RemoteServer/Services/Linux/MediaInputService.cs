using System.Diagnostics;

namespace RemoteServer.Services.Linux;

public class MediaInputService : IMediaInput
{
    private const string Ydotool = "ydotool";
    private bool _initialized;

    private void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;
    }

    public void Next() => RunCommand("key KEY_NEXTSONG");
    public void Previous() => RunCommand("key KEY_PREVIOUSSONG");
    public void PlayPause() => RunCommand("key KEY_PLAYPAUSE");
    public void Stop() => RunCommand("key KEY_STOP");
    public void Mute() => RunCommand("key KEY_MUTE");
    public void VolumeUp() => RunCommand("key KEY_VOLUMEUP");
    public void VolumeDown() => RunCommand("key KEY_VOLUMEDOWN");
    public void SetVolume(int level)
    {
        Console.WriteLine($"[LinuxMedia] SetVolume: {level}% (not implemented on Linux)");
    }

    private void RunCommand(string args)
    {
        EnsureInitialized();
        Console.WriteLine($"[LinuxMedia] Executing: ydotool {args}");

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Ydotool,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LinuxMedia] EXCEPTION: {ex.Message}");
        }
    }
}
