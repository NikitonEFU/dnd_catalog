using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public AiController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public record AiRequest(string Prompt);

    [AllowAnonymous]
    [HttpPost("character-help")]
    public async Task<IActionResult> CharacterHelp([FromBody] AiRequest req)
    {
        var prompt = (req?.Prompt ?? "").Trim();
        if (string.IsNullOrEmpty(prompt))
            return BadRequest(new { message = "prompt is required" });

        var apiKey = _config["OPENROUTER_API_KEY"];
        if (string.IsNullOrEmpty(apiKey))
            return StatusCode(500, new { message = "OPENROUTER_API_KEY is missing" });

        // Можешь поменять модель.
        // Часто у OpenRouter есть бесплатные варианты с суффиксом :free (если доступны на твоём аккаунте).
        // Пример: "mistralai/mistral-7b-instruct:free"
        var model = "mistralai/mistral-7b-instruct:free";

        var system = """
Ты — помощник по D&D 5e. Отвечай структурировано:

1) Концепт персонажа (1-2 предложения)
2) Раса / класс / подкласс + почему
3) Характеристики (15/14/13/12/10/8) — распределение
4) Предыстория, навыки, инструменты, языки
5) Снаряжение
6) Боевой план на 1-3 уровня
7) Короткий итог для копипаста

Пиши понятно, без воды. Если чего-то не хватает — задай 2-3 уточняющих вопроса.
""";

        var payload = new
        {
            model = model,
            messages = new object[]
            {
                new { role = "system", content = system },
                new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 700
        };

        var client = _httpClientFactory.CreateClient();

        var http = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
        http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // Optional, но полезно (для OpenRouter атрибуции)
        var siteUrl = _config["OPENROUTER_SITE_URL"] ?? "https://dnd-catalog-frontend.onrender.com";
        var appName = _config["OPENROUTER_APP_NAME"] ?? "DnD Grimoire";
        http.Headers.TryAddWithoutValidation("HTTP-Referer", siteUrl);
        http.Headers.TryAddWithoutValidation("X-OpenRouter-Title", appName);

        http.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var resp = await client.SendAsync(http);
        var json = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            // Вернём сырой ответ, чтобы ты видел причину (лимиты/нет доступа/и т.д.)
            return StatusCode((int)resp.StatusCode, new
            {
                message = "OpenRouter error",
                details = json,
                model
            });
        }

        // OpenAI-совместимый формат: choices[0].message.content
        try
        {
            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            return Ok(new { text = content, model });
        }
        catch
        {
            return Ok(new { text = json, model });
        }
    }
}
