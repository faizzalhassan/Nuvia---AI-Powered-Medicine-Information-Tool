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

        var rawResponse = await _geminiService.GetMedicineInfoAsync(query);

        if (rawResponse == null)
            return StatusCode(503, new { message = "AI service is temporarily unavailable." });

        Console.WriteLine($"Raw response: {rawResponse}");

        try
        {
            var cleaned = rawResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var startIndex = cleaned.IndexOf('{');
            var lastIndex = cleaned.LastIndexOf('}');

            if (startIndex == -1 || lastIndex == -1)
                return StatusCode(500, new { message = "Invalid response format." });

            cleaned = cleaned.Substring(startIndex, lastIndex - startIndex + 1);

            var medicineInfo = JsonSerializer.Deserialize<MedicineInfo>(
                cleaned,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (medicineInfo == null)
                return StatusCode(500, new { message = "Failed to parse medicine information." });

            if (!medicineInfo.Found)
                return NotFound(new { message = medicineInfo.NotFoundMessage });

            return Ok(medicineInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parse error: {ex.Message}");
            return StatusCode(500, new { message = "Failed to process medicine information." });
        }
    }
}