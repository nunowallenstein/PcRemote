namespace RemoteServer.Services;

public interface ITogglable
{
    void Toggle(string target);
    bool IsToggled(string target);
}
