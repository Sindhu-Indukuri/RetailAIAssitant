using System.Text;
using System.Text.Json;

namespace RetailAIAssitant.AI
{
    public class GeminiClient : IAIClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeminiClient(
            HttpClient http,
            IConfiguration config)
        {
            _http = http;
            _apiKey = config["Gemini:ApiKey"];
        }

        public async Task<string> GenerateAsync(
            string prompt)
        {
            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var response = await _http.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            );

            var json =
                await response.Content.ReadAsStringAsync();

            using var doc =
                JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty(
                "candidates",
                out var candidates))
            {
                return json;
            }

            return candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }
    }
}