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

    public async Task<string?> GetMedicineInfoAsync(string query)
    {
        var prompt = $"You are a medicine information assistant. The user searched for: \"{query}\".\n\n" +
"Return ONLY this JSON object. Do not return markdown, explanations, or any text outside the JSON:\n" +
    "{\"found\":true,\"brandName\":\"\",\"genericName\":\"\",\"drugClass\":\"\",\"manufacturer\":\"\",\"strength\":\"\",\"form\":\"\",\"route\":\"\",\"whatIsIt\":\"\",\"usedFor\":[],\"sideEffects\":[],\"warnings\":[],\"directions\":[],\"relatedVariants\":[],\"disclaimer\":\"This information is for educational purposes only. Always consult a doctor or pharmacist.\",\"notFoundMessage\":\"\"}\n\n" +

    "CONTENT RULES:\n" +

    "- found: true only when the searched medicine can be reasonably identified. If it cannot be confidently identified, set found to false.\n" +

    "- brandName: The commonly recognized brand name.\n" +

    "- genericName: The active/generic ingredient.\n" +

    "- drugClass: A simple category such as Pain reliever, Antibiotic, Antihistamine, Antacid, or Anti-inflammatory.\n" +

    "- manufacturer: Provide only when confidently known for the identified product. Never guess. If uncertain, return an empty string.\n" +

    "- strength: Give the exact strength of the identified product. Never guess or infer a strength.\n" +

    "- form: Must be one of: Tablet, Capsule, Syrup, Gel, Oral Gel, Cream, Drops, Inhaler, Injection, Spray, Patch.\n" +

    "- route: Use a simple route such as Oral, Topical, Nasal, Inhaled, or Injection.\n" +

    "- whatIsIt: Write 1-2 simple sentences explaining what the medicine contains and what it generally does. Avoid unnecessary medical terminology.\n" +

    "- usedFor: Maximum 5 common uses or symptoms. Use simple terms such as Headache, Fever, Toothache, Allergies, or Stomach acid. Do not invent indications.\n" +

    "- sideEffects: Maximum 5 possible side effects. Prefer common effects first. Do not present rare serious reactions as common side effects. Never invent side effects.\n" +

    "- warnings: Maximum 5 important warnings relevant to this medicine. Include overdose, allergies, pregnancy, liver/kidney problems, interactions, stomach problems, or age restrictions only when applicable.\n" +

    "- directions: Maximum 5 steps. Give clear general directions including dose, time between doses, and maximum daily amount ONLY when reliably known.\n" +

    "- Dosage must match the exact identified strength, form, route, age group, and formulation. Never use the dosage of another strength or formulation.\n" +

    "- Never invent a dosage. If the dosage depends on age, weight, condition, or prescription instructions, tell the user to follow their doctor, pharmacist, or package instructions.\n" +

    "- Never recommend exceeding a known maximum daily dose.\n" +

    "- relatedVariants: Maximum 3 verified variants of the SAME brand. Never guess variants. Never infer variants from the generic ingredient. If no variants are confidently known, return an empty array.\n" +

    "- Each relatedVariant must contain: {\"name\":\"\",\"form\":\"\",\"strength\":\"\"}.\n" +

    "- disclaimer: Always return exactly: 'This information is for educational purposes only. Always consult a doctor or pharmacist.'\n" +

    "- notFoundMessage: If found is false, briefly ask the user to check the spelling or provide the exact medicine name.\n" +

    "- If found is false, leave all medicine fields empty except found, disclaimer, and notFoundMessage.\n\n" +

    "QUALITY RULES:\n" +

    "- Accuracy is more important than completeness.\n" +

    "- Never invent medical facts, manufacturers, dosage, strengths, indications, side effects, or variants.\n" +

    "- If information is uncertain, leave the field empty or give safe advice to consult a doctor or pharmacist.\n" +

    "- Keep the language simple and understandable.\n" +

    "- Do not diagnose the user or claim that a medicine is definitely suitable for them.\n" +

    "- Do not claim that a medicine is safe for everyone.\n" +

    "- Return valid JSON only. Do not use markdown.\n" +

    "- Ensure all strings are properly escaped and the JSON can be parsed directly.\n\n" +

    "Example for Panadol 500 mg:\n" +

    "{\"found\":true,\"brandName\":\"Panadol\",\"genericName\":\"Paracetamol\",\"drugClass\":\"Pain reliever & fever reducer\",\"manufacturer\":\"\",\"strength\":\"500 mg\",\"form\":\"Tablet\",\"route\":\"Oral\",\"whatIsIt\":\"Panadol contains paracetamol, a medicine used to relieve mild to moderate pain and reduce fever.\",\"usedFor\":[\"Headache\",\"Fever\",\"Toothache\",\"Sore throat\",\"General aches and pains\"],\"sideEffects\":[\"Nausea\",\"Stomach discomfort\",\"Skin rash\"],\"warnings\":[\"Do not exceed the recommended dose\",\"Do not take with other medicines containing paracetamol\",\"Ask a doctor or pharmacist if you have liver or kidney problems\",\"Seek medical help if too much is taken\"],\"directions\":[\"Take 1-2 tablets (500-1000 mg) per dose\",\"Leave at least 4 hours between doses\",\"Do not take more than 8 tablets (4000 mg) in 24 hours\",\"Do not combine with other medicines containing paracetamol\"],\"relatedVariants\":[],\"disclaimer\":\"This information is for educational purposes only. Always consult a doctor or pharmacist.\",\"notFoundMessage\":\"\"}\n\n" +

    "Now generate the JSON for the user's searched medicine.";

        if (string.IsNullOrEmpty(_apiKey))
        {
            Console.WriteLine("Gemini API key is missing.");
            return null;
        }

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
                maxOutputTokens = 1000
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={_apiKey}";

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