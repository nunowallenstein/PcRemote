using System.Runtime.InteropServices;

namespace RemoteServer.Services.Windows.KeyboardInput;

public class KeyPressService : IKeyPressService
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const uint KEYEVENTF_KEYUP = 0x0002;

    private const byte VK_SHIFT = 0x10;
    private const byte VK_CONTROL = 0x11;
    private const byte VK_MENU = 0x12;
    private const byte VK_LWIN = 0x5B;
    private const byte VK_NUMLOCK = 0x90;
    private const byte VK_DELETE = 0x2E;

    private readonly Dictionary<string, bool> _toggledStates = new()
    {
        ["shift"] = false,
        ["ctrl"] = false,
        ["alt"] = false,
        ["win"] = false,
        ["numlock"] = false,
        ["delete"] = false
    };

    public void KeyPress(string key)
    {
        Console.WriteLine($"[KeyPressService] KeyPress called with key='{key}', holdDuration not provided");
        var vk = KeyMapper.GetVkFromKeyName(key);

        if (vk != 0)
        {
            Console.WriteLine($"[KeyPressService] KeyPress: {key} (vk=0x{vk:X2})");
            keybd_event((byte)vk, 0, 0, 0);
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);
            Console.WriteLine("[KeyPressService] KeyPress completed");
        }
        else
        {
            Console.WriteLine($"[KeyPressService] KeyPress: Unknown key '{key}'");
        }
    }

    public void Toggle(string target)
    {
        var key = target.ToLower();
        if (!_toggledStates.ContainsKey(key))
            return;

        var isCurrentlyToggled = _toggledStates[key];

        if (isCurrentlyToggled)
        {
            Console.WriteLine($"[KeyPressService] {key} Toggle OFF");
            keybd_event(GetVk(key), 0, KEYEVENTF_KEYUP, 0);
            _toggledStates[key] = false;
        }
        else
        {
            Console.WriteLine($"[KeyPressService] {key} Toggle ON");
            keybd_event(GetVk(key), 0, 0, 0);
            _toggledStates[key] = true;
        }
    }

    public bool IsToggled(string target)
    {
        var key = target.ToLower();
        var vk = GetVk(key);

        if (vk == 0)
            return false;

        var isPressed = (GetAsyncKeyState(vk) & 0x8000) != 0;

        _toggledStates[key] = isPressed;

        return isPressed;
    }

    private static byte GetVk(string key)
    {
        return key switch
        {
            "shift" => VK_SHIFT,
            "ctrl" => VK_CONTROL,
            "alt" => VK_MENU,
            "win" => VK_LWIN,
            "numlock" => VK_NUMLOCK,
            "delete" => VK_DELETE,
            _ => (byte)KeyMapper.GetVkFromKeyName(key)
        };
    }
}
