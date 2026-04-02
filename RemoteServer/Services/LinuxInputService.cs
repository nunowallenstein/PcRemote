namespace RemoteServer.Services;

public class LinuxInputService : IInputService
{
    private readonly string _ydotool;

    public LinuxInputService(string ydotoolPath = "ydotool")
    {
        _ydotool = ydotoolPath;
    }

    public void MediaNext() => Run("key KEY_NEXTSONG");
    public void MediaPrevious() => Run("key KEY_PREVIOUSSONG");
    public void MediaPlayPause() => Run("key KEY_PLAYPAUSE");
    public void MediaStop() => Run("key KEY_STOP");
    public void MediaMute() => Run("key KEY_MUTE");
    public void MediaVolumeUp() => Run("key KEY_VOLUMEUP");
    public void MediaVolumeDown() => Run("key KEY_VOLUMEDOWN");

    public void MouseMove(int dx, int dy) => Run(_ydotool, $"mousemove --relative {dx} {dy}");
    public void MouseLeftClick() => Run(_ydotool, "click 0xC0");
    public void MouseRightClick() => Run(_ydotool, "click 0xC1");
    public void MouseScroll(int dy) => Run(_ydotool, $"mousemove --relative 0 {dy}");

    public void KeyPress(string key)
    {
        var k = key.ToLower() switch
        {
            "enter" => "KEY_ENTER",
            "escape" => "KEY_ESC",
            "tab" => "KEY_TAB",
            "backspace" => "KEY_BACKSPACE",
            "space" => "KEY_SPACE",
            "up" => "KEY_UP",
            "down" => "KEY_DOWN",
            "left" => "KEY_LEFT",
            "right" => "KEY_RIGHT",
            "home" => "KEY_HOME",
            "end" => "KEY_END",
            "pageup" => "KEY_PAGEUP",
            "pagedown" => "KEY_PAGEDOWN",
            _ => null
        };

        if (k != null)
            Run(_ydotool, $"key {k}");
    }

    private void Run(string args)
    {
        Run(_ydotool, args);
    }

    private void Run(string cmd, string args)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = System.Diagnostics.Process.Start(startInfo);
            process?.WaitForExit(1000);
        }
        catch { }
    }
}
