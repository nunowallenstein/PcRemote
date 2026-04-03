using System.Runtime.InteropServices;

namespace RemoteServer.Services;

public class WindowsMouseInputService : IMouseInput
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;

    public void Move(int dx, int dy)
    {
        Console.WriteLine($"[WindowsMouse] Move: dx={dx}, dy={dy}");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsMouse] Current position: {point.X}, {point.Y}");
            var newX = point.X + dx;
            var newY = point.Y + dy;
            Console.WriteLine($"[WindowsMouse] Moving to: {newX}, {newY}");
            
            var result = SetCursorPos(newX, newY);
            Console.WriteLine($"[WindowsMouse] SetCursorPos result: {result}");
        }
        else
        {
            Console.WriteLine("[WindowsMouse] GetCursorPos failed");
        }
    }

    public void Click()
    {
        Console.WriteLine("[WindowsMouse] Click");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsMouse] Clicking at: {point.X}, {point.Y}");
            SetCursorPos(point.X, point.Y);
            
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsMouse] Click completed");
        }
        else
        {
            Console.WriteLine("[WindowsMouse] GetCursorPos failed");
        }
    }

    public void RightClick()
    {
        Console.WriteLine("[WindowsMouse] RightClick");
        
        if (GetCursorPos(out var point))
        {
            Console.WriteLine($"[WindowsMouse] Clicking at: {point.X}, {point.Y}");
            SetCursorPos(point.X, point.Y);
            
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            
            Console.WriteLine("[WindowsMouse] RightClick completed");
        }
        else
        {
            Console.WriteLine("[WindowsMouse] GetCursorPos failed");
        }
    }

    public void Scroll(int dy)
    {
        Console.WriteLine($"[WindowsMouse] Scroll: dy={dy}");
        mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)dy, 0);
        Console.WriteLine("[WindowsMouse] Scroll completed");
    }

    public void LeftDown()
    {
        Console.WriteLine("[WindowsMouse] LeftDown");
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
    }

    public void LeftUp()
    {
        Console.WriteLine("[WindowsMouse] LeftUp");
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}