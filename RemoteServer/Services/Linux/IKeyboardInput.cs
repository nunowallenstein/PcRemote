namespace RemoteServer.Services.Linux;

public interface IKeyboardInput
{
    void KeyPress(string key);
    void TypeText(string text);
    void Toggle(string target);
    bool IsToggled(string target);
}
