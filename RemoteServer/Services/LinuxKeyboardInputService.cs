using System.Diagnostics;

namespace RemoteServer.Services;

public class LinuxKeyboardInputService : IKeyboardInput
{
    private readonly string _ydotool;
    private readonly string _ydotoold;
    private bool _initialized;
    private bool _initializationFailed;

    public LinuxKeyboardInputService(string ydotoolPath = "ydotool", string ydotooldPath = "ydotoold")
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
                Console.WriteLine("[LinuxKeyboard] ERROR: ydotoold not found. Install with: sudo apt install ydotoold");
                _initializationFailed = true;
                return;
            }
        }
        catch
        {
            Console.WriteLine("[LinuxKeyboard] ERROR: ydotoold not found. Install with: sudo apt install ydotoold");
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
            Console.WriteLine($"[LinuxKeyboard] Started ydotoold (PID: {daemonProcess?.Id})");
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

    public void KeyPress(string key)
    {
        EnsureInitialized();
        var k = LinuxKeyMapper.GetLinuxKeyFromKeyName(key);

        if (k != null)
            RunCommand($"key {k}");
    }

    public void TypeText(string text)
    {
        EnsureInitialized();
        Console.WriteLine($"[LinuxKeyboard] TypeText: {text}");
        
        foreach (char c in text)
        {
            bool needsShift = char.IsUpper(c) || KeyCharHelper.IsShiftedChar(c);
            
            if (needsShift)
            {
                RunCommand("key KEY_LEFTSHIFT:1");
            }
            
            var escapedText = c.ToString().Replace("'", "'\\''");
            RunCommand($"text '{escapedText}'");
            
            if (needsShift)
            {
                RunCommand("key KEY_LEFTSHIFT:0");
            }
            
            Thread.Sleep(10);
        }
        
        Console.WriteLine("[LinuxKeyboard] TypeText completed");
    }

    private void RunCommand(string args)
    {
        var cmd = $"{_ydotool} {args}";
        Console.WriteLine($"[LinuxKeyboard] Executing: {cmd}");

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _ydotool,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                Console.WriteLine("[LinuxKeyboard] ERROR: Failed to start process");
                return;
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit(1000);

            Console.WriteLine($"[LinuxKeyboard] Exit code: {process.ExitCode}");
            
            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine($"[LinuxKeyboard] Output: {output.Trim()}");
            
            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine($"[LinuxKeyboard] Error: {error.Trim()}");

            if (process.ExitCode != 0)
                Console.WriteLine($"[LinuxKeyboard] Command failed with exit code {process.ExitCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LinuxKeyboard] EXCEPTION: {ex.GetType().Name}: {ex.Message}");
        }
        
        Console.WriteLine("[LinuxKeyboard] Done");
    }
}