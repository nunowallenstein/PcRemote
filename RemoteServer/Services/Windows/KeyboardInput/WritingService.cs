using RemoteServer.Services.Windows.KeyboardInput.TextStrategies;

namespace RemoteServer.Services.Windows.KeyboardInput;

public class WritingService : IWritingService
{
    private readonly ITextStrategy _normalStrategy;
    private readonly ITextStrategy _specialStrategy;

    public WritingService()
    {
        _normalStrategy = new KeybdTextStrategy();
        _specialStrategy = new SendKeysTextStrategy();
    }

    public void TypeText(string text)
    {
        foreach (char c in text)
        {
            // Use SendKeys for special chars (accented, symbols), Keybd for normal chars
            if (c > 127 || KeyMapper.NeedsAltNumpad(c))
            {
                _specialStrategy.TrySendChar(c);
            }
            else
            {
                _normalStrategy.TrySendChar(c);
            }
        }
    }
}
