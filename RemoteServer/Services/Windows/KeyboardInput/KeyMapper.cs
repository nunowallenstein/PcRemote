namespace RemoteServer.Services.Windows.KeyboardInput;

public static class KeyMapper
{
    public static short GetVkFromChar(char c)
    {
        // Handle shifted chars first (before lowercasing)
        var vk = c switch
        {
            '!' => (short)0x31,
            '@' => (short)0x32,
            '#' => (short)0x33,
            '$' => (short)0x34,
            '%' => (short)0x35,
            '^' => (short)0x36,
            '&' => (short)0x37,
            '*' => (short)0x38,
            '(' => (short)0x39,
            ')' => (short)0x30,
            '_' => (short)0xBD,
            '+' => (short)0xBB,
            '{' => (short)0xDB,
            '}' => (short)0xDD,
            '|' => (short)0xDC,
            ':' => (short)0xBA,
            '"' => (short)0xDE,
            '<' => (short)0xBC,
            '>' => (short)0xBE,
            '?' => (short)0xBF,
            '~' => (short)0xC0,
            _ => (short)0
        };

        if (vk != 0)
            return vk;

        // Lowercase the rest and lookup
        c = char.ToLower(c);
        return c switch
        {
            'a' => 0x41,
            'b' => 0x42,
            'c' => 0x43,
            'd' => 0x44,
            'e' => 0x45,
            'f' => 0x46,
            'g' => 0x47,
            'h' => 0x48,
            'i' => 0x49,
            'j' => 0x4A,
            'k' => 0x4B,
            'l' => 0x4C,
            'm' => 0x4D,
            'n' => 0x4E,
            'o' => 0x4F,
            'p' => 0x50,
            'q' => 0x51,
            'r' => 0x52,
            's' => 0x53,
            't' => 0x54,
            'u' => 0x55,
            'v' => 0x56,
            'w' => 0x57,
            'x' => 0x58,
            'y' => 0x59,
            'z' => 0x5A,
            '0' => 0x30,
            '1' => 0x31,
            '2' => 0x32,
            '3' => 0x33,
            '4' => 0x34,
            '5' => 0x35,
            '6' => 0x36,
            '7' => 0x37,
            '8' => 0x38,
            '9' => 0x39,
            ' ' => 0x20,
            '-' => 0xBD,
            '=' => 0xBB,
            '[' => 0xDB,
            ']' => 0xDD,
            '\\' => 0xDC,
            ';' => 0xBA,
            '\'' => 0xDE,
            ',' => 0xBC,
            '.' => 0xBE,
            '/' => 0xBF,
            '`' => 0xC0,
            _ => (short)0
        };
    }

    public static bool NeedsAltNumpad(char c)
    {
        return !Char.IsAsciiLetterOrDigit(c);
    }

    public static short GetVkFromKeyName(string key)
    {
        return key.ToLower() switch
        {
            "enter" => 0x0D,
            "escape" => 0x1B,
            "tab" => 0x09,
            "backspace" => 0x08,
            "space" => 0x20,
            "up" => 0x26,
            "down" => 0x28,
            "left" => 0x25,
            "right" => 0x27,
            "home" => 0x24,
            "end" => 0x23,
            "pageup" => 0x21,
            "pagedown" => 0x22,
            "windows" => 0x5B,
            "win" => 0x5B,
            "alt" => 0xA4,
            "ctrl" => 0xA2,
            "shift" => 0xA0,
            "delete" => 0x2E,
            "insert" => 0x2D,
            _ => 0
        };
    }

    public static byte GetMediaVk(string name)
    {
        return name.ToLower() switch
        {
            "next" => 0xB0,
            "prev" => 0xB1,
            "play" => 0xB3,
            "pause" => 0xB3,
            "stop" => 0xB2,
            "mute" => 0xAD,
            "volup" => 0xAF,
            "voldown" => 0xAE,
            _ => 0
        };
    }
}
