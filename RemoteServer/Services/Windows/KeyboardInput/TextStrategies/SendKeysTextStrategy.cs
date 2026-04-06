using System.Windows.Forms;

namespace RemoteServer.Services.Windows.KeyboardInput.TextStrategies;

public class SendKeysTextStrategy : ITextStrategy
{
    public bool TrySendChar(char c)
    {
        Console.WriteLine($"[SendKeysTextStrategy] SendChar: '{c}'");

        try
        {
            var escaped = EscapeForSendKeys(c.ToString());
            Console.WriteLine($"[SendKeysTextStrategy] Sending via SendKeys: '{escaped}'");
            SendKeys.SendWait(escaped);
            Console.WriteLine($"[SendKeysTextStrategy] Success for '{c}'");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SendKeysTextStrategy] Error for '{c}': {ex.Message}");
            return false;
        }
    }

    internal static string EscapeForSendKeys(string text)
    {
        return text
            .Replace("{", "{{")
            .Replace("}", "{}}")
            .Replace("+", "{+}")
            .Replace("^", "{^}")
            .Replace("~", "{~}")
            .Replace("(", "{(}")
            .Replace(")", "{)}");
    }

    public void SendText(string text)
    {
        Console.WriteLine($"[SendKeysTextStrategy] SendText: {text}");

        foreach (char c in text)
        {
            TrySendChar(c);
        }

        Console.WriteLine("[SendKeysTextStrategy] SendText completed");
    }
}
