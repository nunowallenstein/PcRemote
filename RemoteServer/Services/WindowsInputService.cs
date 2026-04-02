using System.Runtime.InteropServices;

namespace RemoteServer.Services;

public class WindowsInputService : IInputService
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    public void MediaNext() => keybd_event(0xB0, 0, 0, 0);
    public void MediaPrevious() => keybd_event(0xB1, 0, 0, 0);
    public void MediaPlayPause() => keybd_event(0xB3, 0, 0, 0);
    public void MediaStop() => keybd_event(0xB2, 0, 0, 0);
    public void MediaMute() => keybd_event(0xAD, 0, 0, 0);
    public void MediaVolumeUp() => keybd_event(0xAF, 0, 0, 0);
    public void MediaVolumeDown() => keybd_event(0xAE, 0, 0, 0);

    public void MouseMove(int dx, int dy)
    {
        if (GetCursorPos(out var point))
            SetCursorPos(point.X + dx, point.Y + dy);
    }

    public void MouseLeftClick()
    {
        if (GetCursorPos(out var point))
        {
            SetCursorPos(point.X, point.Y);
            mouse_event(0x0002, 0, 0, 0, 0);  // LEFTDOWN
            mouse_event(0x0004, 0, 0, 0, 0);  // LEFTUP
        }
    }

    public void MouseRightClick()
    {
        if (GetCursorPos(out var point))
        {
            SetCursorPos(point.X, point.Y);
            mouse_event(0x0008, 0, 0, 0, 0);  // RIGHTDOWN
            mouse_event(0x0010, 0, 0, 0, 0);  // RIGHTUP
        }
    }

    public void MouseScroll(int dy) => mouse_event(0x0800, 0, 0, dy, 0);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

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
            keybd_event((byte)vk, 0, 0, 0);
    }
}
