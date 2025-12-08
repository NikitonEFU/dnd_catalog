// Services/AuthService.cs (ОНОВЛЕНО)

using dnd_catalog_backend.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using BCrypt.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace dnd_catalog_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public AuthService(
            IMongoDatabase database,
            IOptions<DndDatabaseSettings> dndDatabaseSettings,
            IConfiguration configuration)
        {
            // Налаштування для MongoDB
            var settings = dndDatabaseSettings.Value;
            _usersCollection = database.GetCollection<User>("Users"); // Ім'я колекції

            // Налаштування для JWT
            _jwtKey = configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key not found.");
            _jwtIssuer = configuration["JwtSettings:Issuer"] ?? "defaultIssuer";
            _jwtAudience = configuration["JwtSettings:Audience"] ?? "defaultAudience";
        }

        // --- ЛОГІКА АВТЕНТИФІКАЦІЇ ---

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User?> Authenticate(string username, string password)
        {
            var user = await GetUserByUsername(username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // Неправильний логін або пароль
            }

            return user; // Успіх
        }

        public async Task<User> Register(string username, string password)
        {
            if (await GetUserByUsername(username) != null)
            {
                throw new InvalidOperationException("User with this username already exists.");
            }

            // 1. Хешуємо пароль
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Role = "user"
            };

            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        // --- ЛОГІКА ГЕНЕРАЦІЇ ТОКЕНА ---

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id), // ID користувача
                new Claim(ClaimTypes.Role, user.Role) // РОЛЬ користувача
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Токен дійсний 7 днів
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // --- НОВА РЕАЛІЗАЦІЯ: ОТРИМАННЯ ВСІХ КОРИСТУВАЧІВ ---
        public async Task<List<User>> GetAllUsersAsync()
        {
            // Отримати всіх користувачів з колекції
            return await _usersCollection.Find(_ => true).ToListAsync();
        }
    }
}