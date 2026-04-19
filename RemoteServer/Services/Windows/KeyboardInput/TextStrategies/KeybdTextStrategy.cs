using System.Runtime.InteropServices;

namespace RemoteServer.Services.Windows.KeyboardInput.TextStrategies;

public class KeybdTextStrategy : ITextStrategy
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const byte VK_SHIFT = 0x10;

    public bool TrySendChar(char c)
    {
        var vk = KeyMapper.GetVkFromChar(c);
        Console.WriteLine($"[KeybdTextStrategy] SendChar: '{c}', vk=0x{vk:X2}");

        if (vk == 0)
        {
            Console.WriteLine($"[KeybdTextStrategy] No VK for char '{c}', skipping");
            return false;
        }

        bool needsShift = char.IsUpper(c) || KeyCharHelper.IsShiftedChar(c);

        if (needsShift)
        {
            keybd_event(VK_SHIFT, 0, 0, 0);
        }

        keybd_event((byte)vk, 0, 0, 0);
        keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);

        if (needsShift)
        {
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
        }

        return true;
    }
}
