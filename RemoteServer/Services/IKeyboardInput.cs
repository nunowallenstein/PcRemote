namespace RemoteServer.Services;

public interface IKeyboardInput
{
    void KeyPress(string key);
    void TypeText(string text);
}