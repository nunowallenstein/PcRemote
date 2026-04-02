using System.Runtime.InteropServices;

namespace RemoteServer.Services;

public class WindowsInputService : IInputService
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;

    public void MediaNext() => SendMediaKey(0xB0, "MEDIA_NEXT_TRACK");
    public void MediaPrevious() => SendMediaKey(0xB1, "MEDIA_PREV_TRACK");
    public void MediaPlayPause() => SendMediaKey(0xB3, "MEDIA_PLAY_PAUSE");
    public void MediaStop() => SendMediaKey(0xB2, "MEDIA_STOP");
    public void MediaMute() => SendMediaKey(0xAD, "VOLUME_MUTE");
    public void MediaVolumeUp() => SendMediaKey(0xAF, "VOLUME_UP");
    public void MediaVolumeDown() => SendMediaKey(0xAE, "VOLUME_DOWN");

    public void MouseMove(int dx, int dy)
    {
        Console.WriteLine($"[WindowsInput] MouseMove: dx={dx}, dy={dy}");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsInput] Current cursor position: {point.X}, {point.Y}");
            var newX = point.X + dx;
            var newY = point.Y + dy;
            Console.WriteLine($"[WindowsInput] Moving to: {newX}, {newY}");
            
            var result = SetCursorPos(newX, newY);
            Console.WriteLine($"[WindowsInput] SetCursorPos result: {result}");
        }
        else
        {
            Console.WriteLine("[WindowsInput] GetCursorPos failed");
        }
    }

    public void MouseLeftClick()
    {
        Console.WriteLine("[WindowsInput] MouseLeftClick");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsInput] Clicking at: {point.X}, {point.Y}");
            SetCursorPos(point.X, point.Y);
            
            Console.WriteLine("[WindowsInput] Sending LEFTDOWN");
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsInput] Sending LEFTUP");
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsInput] Left click completed");
        }
        else
        {
            Console.WriteLine("[WindowsInput] GetCursorPos failed");
        }
    }

    public void MouseRightClick()
    {
        Console.WriteLine("[WindowsInput] MouseRightClick");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsInput] Clicking at: {point.X}, {point.Y}");
            SetCursorPos(point.X, point.Y);
            
            Console.WriteLine("[WindowsInput] Sending RIGHTDOWN");
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsInput] Sending RIGHTUP");
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsInput] Right click completed");
        }
        else
        {
            Console.WriteLine("[WindowsInput] GetCursorPos failed");
        }
    }

    public void MouseScroll(int dy)
    {
        Console.WriteLine($"[WindowsInput] MouseScroll: dy={dy}");
        mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)dy, 0);
        Console.WriteLine("[WindowsInput] Scroll completed");
    }

    public void KeyPress(string key)
    {
        short vk = key.ToLower() switch
        {
            "enter" => 0x0D,
            "escape" => 0x1B,
            "tab" => 0x09,
            "backspace" => 0x08,
            "space" => 0x20,
            "up" => 0x26,
            "down" => 0x28,
            "left" => 0x25,
            "right" => 0x27,
            "home" => 0x24,
            "end" => 0x23,
            "pageup" => 0x21,
            "pagedown" => 0x22,
            _ => 0
        };

        if (vk != 0)
        {
            Console.WriteLine($"[WindowsInput] KeyPress: {key} (vk=0x{vk:X2})");
            keybd_event((byte)vk, 0, 0, 0);
            Console.WriteLine("[WindowsInput] KeyPress completed");
        }
        else
        {
            Console.WriteLine($"[WindowsInput] KeyPress: Unknown key '{key}'");
        }
    }

    private void SendMediaKey(byte vk, string name)
    {
        Console.WriteLine($"[WindowsInput] MediaKey: {name} (vk=0x{vk:X2})");
        keybd_event(vk, 0, 0, 0);
        Console.WriteLine($"[WindowsInput] MediaKey {name} sent");
    }
}
