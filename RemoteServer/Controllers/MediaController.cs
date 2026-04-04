using Microsoft.AspNetCore.Mvc;
using RemoteServer.Services;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaInput _media;

    public MediaController(IMediaInput media)
    {
        _media = media;
    }

    [HttpPost("{cmd}")]
    public IActionResult Media(string cmd)
    {
        Console.WriteLine($"[MediaController] cmd: {cmd}");
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

    [HttpPost("volume")]
    public IActionResult Volume([FromBody] int level)
    {
        Console.WriteLine($"[MediaController] Volume: {level}");
        _media.SetVolume(level);
        return Ok();
    }
}
