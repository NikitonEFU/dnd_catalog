// Controllers/CharacterController.cs (ПОВНИЙ ВИПРАВЛЕНИЙ КОД)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using dnd_catalog_backend.Services;
using dnd_catalog_backend.Models; // Потрібно імпортувати моделі

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    // ВИПРАВЛЕНО: Позначено як readonly для вирішення помилки ініціалізації
    private readonly IDndCatalogService _dndService;

    public CharacterController(IDndCatalogService dndService)
    {
        _dndService = dndService; // Ініціалізація поля в конструкторі
    }

    // POST: api/Character (Створення персонажа)
    [HttpPost]
    public async Task<IActionResult> CreateCharacter(CharacterCreateDto characterDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { Message = "User ID not found in token." });
        }
        string userId = userIdClaim.Value;

        // Створення об'єкта Character на основі DTO та UserId
        var character = new Character
        {
            // Id залишається default, щоб його встановив MongoDB/Service
            UserId = userId,
            Name = characterDto.Name,
            Gender = characterDto.Gender,
            RaceId = characterDto.RaceId,
            ClassId = characterDto.ClassId,
            Stats = characterDto.Stats
        };

        await _dndService.CreateCharacterAsync(character);

        // ВИПРАВЛЕНО: Використовуємо Created(uri, value)
        // Це вирішує помилку "Имя 'GetMyCharacters' не существует в текущем контексте."
        return Created($"/api/Character/my/{character.Id}", character);
    }

    // GET: api/Character/my (Перегляд персонажів поточного користувача)
    [HttpGet("my")]
    public async Task<ActionResult<List<Character>>> GetMyCharacters()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }
        string userId = userIdClaim.Value;

        var characters = await _dndService.GetCharactersByUserIdAsync(userId);
        return Ok(characters);
    }

    // ПРИМІТКА: Для повноти REST API варто додати [HttpGet("{id:length(24)}")] GetCharacterById

    // DELETE: api/Character/{id} (Видалення персонажа)
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var character = await _dndService.GetCharacterByIdAsync(id);

        if (character is null)
        {
            return NotFound();
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || character.UserId != userIdClaim.Value)
        {
            return Forbid();
        }

        await _dndService.RemoveCharacterAsync(id);
        return NoContent();
    }
}