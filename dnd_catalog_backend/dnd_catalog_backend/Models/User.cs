// Models/User.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// Модель для зберігання даних у MongoDB
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("Username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty; // Зберігаємо тільки хеш!

    [BsonElement("Role")]
    public string Role { get; set; } = "user"; // Роль: "user" або "admin"
}

// DTO для вхідних даних (від користувача)
public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}