using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { new { Name = "Test Plugin" } });
}