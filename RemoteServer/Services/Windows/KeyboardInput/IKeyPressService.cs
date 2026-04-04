namespace RemoteServer.Services.Windows.KeyboardInput;

public interface IKeyPressService
{
    void KeyPress(string key);
    void Toggle(string target);
    bool IsToggled(string target);
}
