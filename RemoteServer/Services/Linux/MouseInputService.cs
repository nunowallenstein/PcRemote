using System.Diagnostics;

namespace RemoteServer.Services.Linux;

public class MouseInputService : IMouseInput
{
    private const string Ydotool = "ydotool";

    private bool _initialized;

    private readonly Dictionary<string, bool> _toggledStates = new()
    {
        ["drag"] = false
    };

    private void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;
    }

    public void Move(int dx, int dy)
    {
        EnsureInitialized();
        RunCommand($"mousemove --relative {dx} {dy}");
    }

    public void Click()
    {
        EnsureInitialized();
        RunCommand("click 0xC0");
    }

    public void RightClick()
    {
        EnsureInitialized();
        RunCommand("click 0xC1");
    }

    public void Scroll(int dy)
    {
        EnsureInitialized();
        RunCommand($"mousemove --relative 0 {dy}");
    }

    public void LeftDown()
    {
        EnsureInitialized();
        RunCommand("click 0x110");
    }

    public void LeftUp()
    {
        EnsureInitialized();
        RunCommand("click 0x111");
    }

    public void Toggle(string target)
    {
        var key = target.ToLower();
        if (!_toggledStates.ContainsKey(key))
            return;

        EnsureInitialized();
        var isCurrentlyToggled = _toggledStates[key];

        if (isCurrentlyToggled)
        {
            Console.WriteLine($"[LinuxMouse] {key} Toggle OFF");
            RunCommand("click 0x111");
            _toggledStates[key] = false;
        }
        else
        {
            Console.WriteLine($"[LinuxMouse] {key} Toggle ON");
            RunCommand("click 0x110");
            _toggledStates[key] = true;
        }
    }

    public bool IsToggled(string target)
    {
        var key = target.ToLower();
        return _toggledStates.TryGetValue(key, out var isToggled) && isToggled;
    }

    private void RunCommand(string args)
    {
        Console.WriteLine($"[LinuxMouse] Executing: ydotool {args}");

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
            Console.WriteLine($"[LinuxMouse] EXCEPTION: {ex.Message}");
        }
    }
}
