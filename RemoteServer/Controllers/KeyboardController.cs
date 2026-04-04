using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models;
using RemoteServer.Services.Windows.KeyboardInput;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api/keyboard")]
public class KeyboardController : ControllerBase
{
    private readonly IKeyPressService _keyPress;
    private readonly IWritingService _writing;

    public KeyboardController(IKeyPressService keyPress, IWritingService writing)
    {
        _keyPress = keyPress;
        _writing = writing;
    }

    [HttpPost]
    public async Task<IActionResult> Keyboard()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[KeyboardController] JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<KeyboardCommand>(json);

        if (!string.IsNullOrEmpty(cmd?.Key))
        {
            var isLongPress = cmd.HoldDuration >= 1000;
            var isQuickUntoggle = cmd.HoldDuration == 0;
            var keyLower = cmd.Key.ToLower();

            var isModifier = keyLower is "shift" or "ctrl" or "alt" or "windows" or "win" or "delete";

            if (isLongPress)
            {
                if (isModifier)
                {
                    _keyPress.Toggle(keyLower == "windows" ? "win" : keyLower);
                }
                else
                {
                    _keyPress.KeyPress(cmd.Key);
                }
            }
            else if (isQuickUntoggle && isModifier && _keyPress.IsToggled(keyLower == "windows" ? "win" : keyLower))
            {
                _keyPress.Toggle(keyLower == "windows" ? "win" : keyLower);
            }
            else
            {
                _keyPress.KeyPress(cmd.Key);
            }
        }

        return Ok();
    }

    [HttpPost("text")]
    public async Task<IActionResult> KeyboardText()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[KeyboardController] Text JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<TextCommand>(json);

        if (!string.IsNullOrEmpty(cmd?.Text))
            _writing.TypeText(cmd.Text);

        return Ok();
    }
}
