namespace RemoteServer.Services;

public static class LinuxKeyMapper
{
    public static string GetLinuxKeyFromKeyName(string key)
    {
        return key.ToLower() switch
        {
            "enter" => "KEY_ENTER",
            "escape" => "KEY_ESC",
            "tab" => "KEY_TAB",
            "backspace" => "KEY_BACKSPACE",
            "space" => "KEY_SPACE",
            "up" => "KEY_UP",
            "down" => "KEY_DOWN",
            "left" => "KEY_LEFT",
            "right" => "KEY_RIGHT",
            "home" => "KEY_HOME",
            "end" => "KEY_END",
            "pageup" => "KEY_PAGEUP",
            "pagedown" => "KEY_PAGEDOWN",
            "windows" => "KEY_LEFTMETA",
            "alt" => "KEY_LEFTALT",
            "ctrl" => "KEY_LEFTCTRL",
            "shift" => "KEY_LEFTSHIFT",
            _ => null
        };
    }

    public static string GetLinuxMediaKey(string name)
    {
        return name.ToLower() switch
        {
            "next" => "KEY_NEXTSONG",
            "prev" => "KEY_PREVIOUSSONG",
            "play" => "KEY_PLAYPAUSE",
            "pause" => "KEY_PLAYPAUSE",
            "stop" => "KEY_STOP",
            "mute" => "KEY_MUTE",
            "volup" => "KEY_VOLUMEUP",
            "voldown" => "KEY_VOLUMEDOWN",
            _ => null
        };
    }
}