using System.Text;
using System.Text.Json;

namespace Nuvia.API.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<string?> GenerateMedicineInfoAsync(string query)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Console.WriteLine("Gemini API key is missing or empty.");
            return null;
        }

        Console.WriteLine($"Gemini API key loaded. Length: {_apiKey.Length}");

        var prompt = $"You are a medicine info assistant. The user searched for: \"{query}\"\n\n" +
    "Reply ONLY with this JSON, no markdown:\n" +
    "{\n" +
    "  \"found\": true,\n" +
    "  \"brandName\": \"\",\n" +
    "  \"genericName\": \"\",\n" +
    "  \"drugClass\": \"\",\n" +
    "  \"manufacturer\": \"\",\n" +
    "  \"strength\": \"\",\n" +
    "  \"form\": \"\",\n" +
    "  \"route\": \"\",\n" +
    "  \"whatIsIt\": \"1 sentence\",\n" +
    "  \"usedFor\": [\"max 4 items\"],\n" +
    "  \"sideEffects\": [\"max 4 items\"],\n" +
    "  \"warnings\": [\"max 3 items\"],\n" +
    "  \"directions\": [\"max 3 items\"],\n" +
    "  \"disclaimer\": \"This information is for educational purposes only. Always consult a doctor.\",\n" +
    "  \"notFoundMessage\": \"\"\n" +
    "}\n" +
    "If medicine not found set found:false and fill notFoundMessage only. Keep all text short and simple.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
{
    temperature = 0.1,
    maxOutputTokens = 800
}
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={_apiKey}";
        Console.WriteLine($"Calling Gemini at: {url.Replace(_apiKey, "***")}");

        try
        {
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Gemini error: {response.StatusCode} - {errorBody}");
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Gemini raw response: {responseJson[..Math.Min(200, responseJson.Length)]}");

            var doc = JsonDocument.Parse(responseJson);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini exception: {ex.Message}");
            return null;
        }
    }
}