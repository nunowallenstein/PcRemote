using System.Diagnostics;

namespace RemoteServer.Services;

public class LinuxInputService : IInputService
{
    private readonly string _ydotool;
    private readonly string _ydotoold;
    private bool _initialized;
    private bool _initializationFailed;

    public LinuxInputService(string ydotoolPath = "ydotool", string ydotooldPath = "ydotoold")
    {
        _ydotool = ydotoolPath;
        _ydotoold = ydotooldPath;
    }

    private void EnsureInitialized()
    {
        if (_initialized || _initializationFailed) return;
        _initialized = true;

        try
        {
            var whichProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "which",
                Arguments = _ydotoold,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            whichProcess?.WaitForExit(2000);
            if (whichProcess?.ExitCode != 0)
            {
                Console.WriteLine("[LinuxInput] ERROR: ydotoold not found. Install with: sudo apt install ydotoold");
                _initializationFailed = true;
                return;
            }
        }
        catch
        {
            Console.WriteLine("[LinuxInput] ERROR: ydotoold not found. Install with: sudo apt install ydotoold");
            _initializationFailed = true;
            return;
        }

        var socketPath = "/tmp/.ydotool_socket";
        if (File.Exists(socketPath))
        {
            try { File.Delete(socketPath); } catch { }
        }

        var checkProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "pgrep",
            Arguments = "-x ydotoold",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        checkProcess?.WaitForExit(2000);
        var isRunning = checkProcess?.ExitCode == 0;

        if (!isRunning)
        {
            RunCommandCore("pkill ydotoold", ignoreError: true);
            Thread.Sleep(200);
            RunCommandCore($"rm -f {socketPath}", ignoreError: true);
            Thread.Sleep(100);

            var daemonProcess = Process.Start(new ProcessStartInfo
            {
                FileName = _ydotoold,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            Console.WriteLine($"[LinuxInput] Started ydotoold (PID: {daemonProcess?.Id})");
            Thread.Sleep(500);
        }
    }

    private void RunCommandCore(string command, bool ignoreError = false)
    {
        try
        {
            var parts = command.Split(' ', 2);
            var startInfo = new ProcessStartInfo
            {
                FileName = parts[0],
                Arguments = parts.Length > 1 ? parts[1] : "",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(startInfo);
            proc?.WaitForExit(3000);
        }
        catch when (ignoreError) { }
    }

    public void MediaNext() { EnsureInitialized(); RunCommand("key KEY_NEXTSONG"); }
    public void MediaPrevious() { EnsureInitialized(); RunCommand("key KEY_PREVIOUSSONG"); }
    public void MediaPlayPause() { EnsureInitialized(); RunCommand("key KEY_PLAYPAUSE"); }
    public void MediaStop() { EnsureInitialized(); RunCommand("key KEY_STOP"); }
    public void MediaMute() { EnsureInitialized(); RunCommand("key KEY_MUTE"); }
    public void MediaVolumeUp() { EnsureInitialized(); RunCommand("key KEY_VOLUMEUP"); }
    public void MediaVolumeDown() { EnsureInitialized(); RunCommand("key KEY_VOLUMEDOWN"); }

    public void MouseMove(int dx, int dy) { EnsureInitialized(); RunCommand($"mousemove --relative {dx} {dy}"); }
    public void MouseLeftClick() { EnsureInitialized(); RunCommand("click 0xC0"); }
    public void MouseRightClick() { EnsureInitialized(); RunCommand("click 0xC1"); }
    public void MouseScroll(int dy) { EnsureInitialized(); RunCommand($"mousemove --relative 0 {dy}"); }

    public void KeyPress(string key)
    {
        EnsureInitialized();
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
            RunCommand($"key {k}");
    }

    private void RunCommand(string args)
    {
        var cmd = $"{_ydotool} {args}";
        Console.WriteLine($"[LinuxInput] Executing: {cmd}");

        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _ydotool,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process == null)
            {
                Console.WriteLine("[LinuxInput] ERROR: Failed to start process");
                return;
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit(1000);

            Console.WriteLine($"[LinuxInput] Exit code: {process.ExitCode}");
            
            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine($"[LinuxInput] Output: {output.Trim()}");
            
            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine($"[LinuxInput] Error: {error.Trim()}");

            if (process.ExitCode != 0)
                Console.WriteLine($"[LinuxInput] Command failed with exit code {process.ExitCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LinuxInput] EXCEPTION: {ex.GetType().Name}: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"[LinuxInput] Inner Exception: {ex.InnerException.Message}");
        }
        
        Console.WriteLine("[LinuxInput] Done");
    }
}
