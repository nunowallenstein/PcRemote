using RemoteServer.Services;
using RemoteServer.Services.Windows.KeyboardInput.TextStrategies;

namespace RemoteServer.Services.Windows.KeyboardInput;

public class WritingService : IWritingService
{
    private readonly ITextStrategy _keybdStrategy;
    private readonly ITextStrategy _sendKeysStrategy;

    public WritingService()
    {
        _keybdStrategy = new KeybdTextStrategy();
        _sendKeysStrategy = new SendKeysTextStrategy();
    }

    public void TypeText(string text)
    {
        foreach (char c in text)
        {
            var strategy = (c > 127 || KeyMapper.NeedsAltNumpad(c))
                ? _sendKeysStrategy
                : _keybdStrategy;

            strategy.TrySendChar(c);
        }
    }
}
