using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models;
using RemoteServer.Services;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api")]
public class RemoteController : ControllerBase
{
    private readonly IKeyboardInput _keyboard;
    private readonly IMouseInput _mouse;
    private readonly IMediaInput _media;

    public RemoteController(IKeyboardInput keyboard, IMouseInput mouse, IMediaInput media)
    {
        _keyboard = keyboard;
        _mouse = mouse;
        _media = media;
    }
    
    [HttpPost("media/{cmd}")]
    public IActionResult Media(string cmd)
    {
        Console.WriteLine($"[RemoteController] Media cmd: {cmd}");
        switch (cmd.ToLower())
        {
            case "next": _media.Next(); break;
            case "prev": _media.Previous(); break;
            case "play": case "pause": _media.PlayPause(); break;
            case "stop": _media.Stop(); break;
            case "mute": _media.Mute(); break;
            case "volup": _media.VolumeUp(); break;
            case "voldown": _media.VolumeDown(); break;
        }
        return Ok();
    }

    [HttpPost("mouse")]
    public async Task<IActionResult> Mouse()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[RemoteController] Mouse JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<MouseCommand>(json);

        switch (cmd?.Action)
        {
            case "move":
                _mouse.Move(cmd.Dx ?? 0, cmd.Dy ?? 0);
                break;
            case "click":
            case "left":
                _mouse.Click();
                break;
            case "leftdown":
                _mouse.LeftDown();
                break;
            case "leftup":
                _mouse.LeftUp();
                break;
            case "rightclick":
            case "right":
                _mouse.RightClick();
                break;
            case "scroll":
                _mouse.Scroll(cmd.Dy ?? 0);
                break;
            default:
                Console.WriteLine($"[RemoteController] Unknown mouse action: {cmd?.Action}");
                break;
        }
        return Ok();
    }

    [HttpPost("keyboard")]
    public async Task<IActionResult> Keyboard()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[RemoteController] Keyboard JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<KeyboardCommand>(json);
        
        if (!string.IsNullOrEmpty(cmd?.Key))
            _keyboard.KeyPress(cmd.Key);

        return Ok();
    }

    [HttpPost("keyboard/text")]
    public async Task<IActionResult> KeyboardText()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[RemoteController] Text JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<TextCommand>(json);
        
        if (!string.IsNullOrEmpty(cmd?.Text))
            _keyboard.TypeText(cmd.Text);

        return Ok();
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return Content(HtmlPage.Content, "text/html");
    }
}