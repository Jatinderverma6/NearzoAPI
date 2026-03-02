using NearzoAPI.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace NearzoAPI.Services.Implementations
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetChatResponseAsync(string message)
        {
            try
            {
                var apiKey = _configuration["AI:ApiKey"];

                var requestBody = new
                {
                    message = message
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://apifreellm.com/api/v1/chat");
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return "AI service failed.";
                }

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<JsonElement>(json);

                return result.GetProperty("response").GetString()?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AI service:: {ex.Message}");
                return $"Error in AI service: {ex.Message}";
            }
        }
    }
}
