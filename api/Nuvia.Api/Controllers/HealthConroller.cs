using Microsoft.AspNetCore.Mvc;

namespace Nuvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            app = "Nuvia API",
            timestamp = DateTime.UtcNow
        });
    }
}