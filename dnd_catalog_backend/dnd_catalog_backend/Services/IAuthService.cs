// Services/IAuthService.cs

using dnd_catalog_backend.Models; // Можливо, вам потрібно змінити namespace
using MongoDB.Driver;

namespace dnd_catalog_backend.Services
{
    public interface IAuthService
    {
        // Повертає користувача, якщо логін успішний, інакше null
        Task<User?> Authenticate(string username, string password);

        // Реєструє нового користувача
        Task<User> Register(string username, string password);

        // Генерує JWT-токен для користувача
        string GenerateJwtToken(User user);

        // Знаходить користувача за ім'ям
        Task<User?> GetUserByUsername(string username);

        Task<List<User>> GetAllUsersAsync();
    }
}