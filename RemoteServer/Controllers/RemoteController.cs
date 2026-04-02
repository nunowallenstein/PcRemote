using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models;
using RemoteServer.Services;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api")]
public class RemoteController : ControllerBase
{
    private readonly IInputService _input;

    public RemoteController(IInputService input)
    {
        _input = input;
    }

    [HttpPost("media/{action}")]
    public IActionResult Media(string action)
    {
        switch (action.ToLower())
        {
            case "next": _input.MediaNext(); break;
            case "prev": _input.MediaPrevious(); break;
            case "play": case "pause": _input.MediaPlayPause(); break;
            case "stop": _input.MediaStop(); break;
            case "mute": _input.MediaMute(); break;
            case "volup": _input.MediaVolumeUp(); break;
            case "voldown": _input.MediaVolumeDown(); break;
        }
        return Ok();
    }

    [HttpPost("mouse")]
    public async Task<IActionResult> Mouse()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        var cmd = System.Text.Json.JsonSerializer.Deserialize<MouseCommand>(json);

        switch (cmd?.Action)
        {
            case "move":
                _input.MouseMove(cmd.Dx ?? 0, cmd.Dy ?? 0);
                break;
            case "click":
                _input.MouseLeftClick();
                break;
            case "rightclick":
                _input.MouseRightClick();
                break;
            case "scroll":
                _input.MouseScroll(cmd.Dy ?? 0);
                break;
        }
        return Ok();
    }

    [HttpPost("keyboard")]
    public async Task<IActionResult> Keyboard()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        var cmd = System.Text.Json.JsonSerializer.Deserialize<KeyboardCommand>(json);
        
        if (!string.IsNullOrEmpty(cmd?.Key))
            _input.KeyPress(cmd.Key);

        return Ok();
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return Content(HtmlPage.Content, "text/html");
    }
}
