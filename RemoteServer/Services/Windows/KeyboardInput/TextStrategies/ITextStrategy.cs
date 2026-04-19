namespace RemoteServer.Services.Windows.KeyboardInput.TextStrategies;

public interface ITextStrategy
{
    bool TrySendChar(char c);
}
