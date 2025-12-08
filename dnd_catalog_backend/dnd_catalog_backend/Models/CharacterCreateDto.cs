// Models/CharacterCreateDto.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Модель для прийому даних від клієнта
public class CharacterCreateDto
{
    [Required] // Додано валідацію, якщо на бекенді немає default
    public required string Name { get; set; }

    [Required]
    public required string Gender { get; set; }

    [Required]
    public required string RaceId { get; set; }

    [Required]
    public required string ClassId { get; set; }

    public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();
}