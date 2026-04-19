namespace RemoteServer.Services;

public interface IMouseInput : ITogglable
{
    void Move(int dx, int dy);
    void Click();
    void RightClick();
    void Scroll(int dy);
    void LeftDown();
    void LeftUp();
}
