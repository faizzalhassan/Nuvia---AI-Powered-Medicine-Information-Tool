using Microsoft.AspNetCore.Mvc;
using Nuvia.API.DTOs;
using Nuvia.API.Services;
using System.Text.Json;

namespace Nuvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly GeminiService _geminiService;

    public MedicinesController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Search query cannot be empty." });

        var rawResponse = await _geminiService.GenerateMedicineInfoAsync(query);

        if (rawResponse == null)
            return StatusCode(503, new { message = "AI service is temporarily unavailable." });

        try
        {
            // Clean response — remove markdown code blocks if present
            var cleaned = rawResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var medicineInfo = JsonSerializer.Deserialize<MedicineInfo>(cleaned, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (medicineInfo == null)
                return StatusCode(500, new { message = "Failed to parse medicine information." });

            if (!medicineInfo.Found)
                return NotFound(new { message = medicineInfo.NotFoundMessage });

            return Ok(medicineInfo);
        }
        catch
        {
            return StatusCode(500, new { message = "Failed to process medicine information." });
        }
    }
}