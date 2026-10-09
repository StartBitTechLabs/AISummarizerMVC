using AISummarizerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AISummarizerMVC.Controllers
{
    public class AIController : Controller
    {
        private readonly HttpClient _httpClient;

        public AIController()
        {
            _httpClient = new HttpClient();
        }

        string apiKey = "use your api key";

        string aiModel = "gpt-5.6-luna";

        string llmApiUrl =
            "https://api.openai.com/v1/chat/completions";

        private async Task<JsonElement> GetAiResponse(string prompt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            var payload = new
            {
                model = aiModel,

                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful AI assistant."
                    },

                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                llmApiUrl,
                payload);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<JsonElement>();

            return result;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generate(string topic)
        {
                string prompt = $"""
                    Analyze the following topic.

                    Generate:
                    1. A concise summary in 2-3 paragraphs
                    2. Exactly 5 meaningful questions

                    Return ONLY valid JSON in this format:

                        "summary": "summary here",
                        "questions": [
                            "question 1",
                            "question 2",
                            "question 3",
                            "question 4",
                            "question 5"
                        ]

                    Topic:
                    {topic}
                    """;

                var result = await GetAiResponse(prompt);

                var aiContent = result
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                var response =
                    JsonSerializer.Deserialize<SummaryQuestionsResponse>(
                        aiContent ?? "{}",
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                var model = new AIResult
                {
                    Topic = topic.ToUpper(),
                    Summary = response?.Summary ?? string.Empty,
                    Questions = response?.Questions ??
                                new List<string>()
                };

                return View(model);
        }
    }
}