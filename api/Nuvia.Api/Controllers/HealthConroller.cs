using Microsoft.AspNetCore.Mvc;
using Nuvia.API.Services;

namespace Nuvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly GeminiService _geminiService;

    public HealthController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

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

    [HttpGet("test-gemini")]
public async Task<IActionResult> TestGemini([FromServices] IConfiguration config)
{
    var key = config["Gemini:ApiKey"];
    Console.WriteLine($"API Key: {(string.IsNullOrEmpty(key) ? "EMPTY - NOT LOADED" : "LOADED - length: " + key.Length)}");

    var result = await _geminiService.GenerateMedicineInfoAsync("panadol");
    if (result == null)
        return StatusCode(503, new { message = "Gemini failed" });
    return Ok(new { raw = result });
}
}