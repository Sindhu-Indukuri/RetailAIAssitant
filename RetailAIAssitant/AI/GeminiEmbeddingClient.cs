using System.Text;
using System.Text.Json;

namespace RetailAIAssitant.AI
{
    public class GeminiEmbeddingClient
        : IEmbeddingClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeminiEmbeddingClient(
            HttpClient http,
            IConfiguration config)
        {
            _http = http;
            _apiKey = config["Gemini:ApiKey"];
        }

        public async Task<float[]>
            GetEmbeddingAsync(string text)
        {
            var body = new
            {
                content = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = text
                        }
                    }
                }
            };

            var response = await _http.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent?key={_apiKey}",
                new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            );

            var json =
    await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            using var doc =
                JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("embedding")
                .GetProperty("values")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
        }
    }
}