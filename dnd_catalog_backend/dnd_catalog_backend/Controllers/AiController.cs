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

        // ✅ Берём HF ключ из env/Render
        var hfKey = _config["HF_API_KEY"];
        if (string.IsNullOrEmpty(hfKey))
            return StatusCode(500, new { message = "HF_API_KEY is missing" });

        // ✅ Модель (если не работает — поменяй на другую)
        var model = "HuggingFaceH4/zephyr-7b-beta";
        var url = $"https://api-inference.huggingface.co/models/{model}";

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

        // Для instruct-моделей хорошо работает формат "System/User"
        var fullPrompt =
$@"[SYSTEM]
{system}

[USER]
{prompt}

[ASSISTANT]
";

        var payload = new
        {
            inputs = fullPrompt,
            parameters = new
            {
                max_new_tokens = 500,
                temperature = 0.7,
                top_p = 0.9,
                return_full_text = false
            },
            options = new
            {
                wait_for_model = true
            }
        };

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hfKey);

        var http = new HttpRequestMessage(HttpMethod.Post, url);
        http.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var resp = await client.SendAsync(http);
        var json = await resp.Content.ReadAsStringAsync();

        // HuggingFace часто возвращает 503 пока модель грузится, или 429 лимит
        if (!resp.IsSuccessStatusCode)
        {
            return StatusCode((int)resp.StatusCode, new
            {
                message = "HuggingFace error",
                details = json,
                model
            });
        }

        // Обычно ответ выглядит так: [ { "generated_text": "..." } ]
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind == JsonValueKind.Array &&
                doc.RootElement.GetArrayLength() > 0 &&
                doc.RootElement[0].TryGetProperty("generated_text", out var gt))
            {
                var text = gt.GetString() ?? "";
                return Ok(new { text, model });
            }

            // Иногда формат другой — вернём сырой json, чтобы увидеть
            return Ok(new { text = json, model });
        }
        catch
        {
            return Ok(new { text = json, model });
        }
    }
}
