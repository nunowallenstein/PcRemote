using Microsoft.AspNetCore.Mvc;
using RemoteServer.Services;
using RemoteServer.Services.Windows.KeyboardInput;

namespace RemoteServer.Controllers;

[ApiController]
[Route("api")]
public class IndexController : ControllerBase
{
    private readonly IKeyPressService _keyPress;
    private readonly IMouseInput _mouse;
    private readonly IWebHostEnvironment _env;

    public IndexController(IKeyPressService keyPress, IMouseInput mouse, IWebHostEnvironment env)
    {
        _keyPress = keyPress;
        _mouse = mouse;
        _env = env;
    }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(new
        {
            keyboard = new
            {
                shift = _keyPress.IsToggled("shift"),
                ctrl = _keyPress.IsToggled("ctrl"),
                alt = _keyPress.IsToggled("alt"),
                win = _keyPress.IsToggled("win"),
                numlock = _keyPress.IsToggled("numlock"),
                delete = _keyPress.IsToggled("delete")
            },
            mouse = new
            {
                drag = _mouse.IsToggled("drag")
            }
        });
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var path = Path.Combine(_env.ContentRootPath, "wwwroot", "index.html");
        var html = await System.IO.File.ReadAllTextAsync(path);
        return Content(html, "text/html");
    }
}
