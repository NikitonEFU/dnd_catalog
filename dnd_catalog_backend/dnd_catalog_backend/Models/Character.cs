// Models/Character.cs (ВИПРАВЛЕНО ДЛЯ C# 11+ ТА required)

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic; // Додаємо для Dictionary

public class Character
{
    // 1. ID: Генерується MongoDB. Використовуємо default!
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    // 2. UserId: Обов'язкове поле (приходить з JWT-токена)
    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string UserId { get; set; }

    // 3. Name, Gender: Обов'язкові поля. 
    // Прибрано "= string.Empty;", оскільки 'required' бере на себе відповідальність за ініціалізацію.
    public required string Name { get; set; }

    public required string Gender { get; set; }

    // 4. Посилання на інші сутності: Обов'язкові поля
    [BsonRepresentation(BsonType.ObjectId)]
    public required string RaceId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public required string ClassId { get; set; }

    // 5. Stats: Словник ініціалізовано за замовчуванням
    public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();
}