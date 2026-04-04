using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models;
using RemoteServer.Services;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api/mouse")]
public class MouseController : ControllerBase
{
    private readonly IMouseInput _mouse;

    public MouseController(IMouseInput mouse)
    {
        _mouse = mouse;
    }

    [HttpPost]
    public async Task<IActionResult> Mouse()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        Console.WriteLine($"[MouseController] JSON: {json}");
        var cmd = System.Text.Json.JsonSerializer.Deserialize<MouseCommand>(json);

        var isLongPress = cmd?.HoldDuration >= 1000;
        var isQuickUntoggle = cmd?.HoldDuration == 0;

        switch (cmd?.Action)
        {
            case "move":
                _mouse.Move(cmd.Dx ?? 0, cmd.Dy ?? 0);
                break;
            case "click":
            case "doubleclick":
                _mouse.Click();
                _mouse.Click();
                break;
            case "left":
                if (isLongPress)
                {
                    _mouse.Toggle("drag");
                }
                else if (isQuickUntoggle && _mouse.IsToggled("drag"))
                {
                    _mouse.Toggle("drag");
                }
                else
                {
                    _mouse.Click();
                }
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
                Console.WriteLine($"[MouseController] Unknown action: {cmd?.Action}");
                break;
        }
        return Ok();
    }
}
