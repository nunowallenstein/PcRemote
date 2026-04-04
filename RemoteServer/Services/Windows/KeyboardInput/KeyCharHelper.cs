namespace RemoteServer.Services.Windows.KeyboardInput;

public static class KeyCharHelper
{
    public static bool IsShiftedChar(char c)
    {
        return c switch
        {
            '!' => true,
            '@' => true,
            '#' => true,
            '$' => true,
            '%' => true,
            '^' => true,
            '&' => true,
            '*' => true,
            '(' => true,
            ')' => true,
            '_' => true,
            '+' => true,
            '{' => true,
            '}' => true,
            '|' => true,
            ':' => true,
            '"' => true,
            '<' => true,
            '>' => true,
            '?' => true,
            '~' => true,
            _ => false
        };
    }
}
