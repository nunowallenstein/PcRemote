using System.Runtime.InteropServices;
using System.Threading;

namespace RemoteServer.Services;

public class WindowsKeyboardInputService : IKeyboardInput
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    private const uint KEYEVENTF_KEYUP = 0x0002;

    public void KeyPress(string key)
    {
        var vk = WindowsKeyMapper.GetVkFromKeyName(key);

        if (vk != 0)
        {
            Console.WriteLine($"[WindowsKeyboard] KeyPress: {key} (vk=0x{vk:X2})");
            keybd_event((byte)vk, 0, 0, 0);
            Console.WriteLine("[WindowsKeyboard] KeyPress completed");
        }
        else
        {
            Console.WriteLine($"[WindowsKeyboard] KeyPress: Unknown key '{key}'");
        }
    }

    public void TypeText(string text)
    {
        Console.WriteLine($"[WindowsKeyboard] TypeText: {text}");
        
        foreach (char c in text)
        {
            bool needsShift = char.IsUpper(c) || KeyCharHelper.IsShiftedChar(c);
            
            if (needsShift)
            {
                keybd_event(0x10, 0, 0, 0);
            }
            
            SendKey(c);
            
            if (needsShift)
            {
                keybd_event(0x10, 0, KEYEVENTF_KEYUP, 0);
            }
            
            Thread.Sleep(10);
        }
        
        Console.WriteLine("[WindowsKeyboard] TypeText completed");
    }

    private void SendKey(char c)
    {
        var vk = WindowsKeyMapper.GetVkFromChar(c);
        if (vk != 0)
        {
            keybd_event((byte)vk, 0, 0, 0);
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}