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

    // Если хочешь доступно всем — раскомментируй и удали [Authorize]
    // [AllowAnonymous]
    [Authorize]
    [HttpPost("character-help")]
    public async Task<IActionResult> CharacterHelp([FromBody] AiRequest req)
    {
        var prompt = (req?.Prompt ?? "").Trim();
        if (string.IsNullOrEmpty(prompt))
            return BadRequest(new { message = "prompt is required" });

        var apiKey = _config["OPENAI_API_KEY"];
        if (string.IsNullOrEmpty(apiKey))
            return StatusCode(500, new { message = "OPENAI_API_KEY is missing" });

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
            model = "gpt-4o-mini",
            input = new object[]
            {
                new { role = "system", content = system },
                new { role = "user", content = prompt }
            }
        };

        var client = _httpClientFactory.CreateClient();
        var http = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        http.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var resp = await client.SendAsync(http);
        var json = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, new { message = "OpenAI error", details = json });

        using var doc = JsonDocument.Parse(json);
        var text = doc.RootElement.GetProperty("output_text").GetString() ?? "";

        return Ok(new { text });
    }
}
