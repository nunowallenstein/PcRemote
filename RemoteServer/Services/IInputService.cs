namespace RemoteServer.Services;

public interface IInputService
{
    void MediaNext();
    void MediaPrevious();
    void MediaPlayPause();
    void MediaStop();
    void MediaMute();
    void MediaVolumeUp();
    void MediaVolumeDown();
    void MouseMove(int dx, int dy);
    void MouseLeftClick();
    void MouseRightClick();
    void MouseScroll(int dy);
    void KeyPress(string key);
}
