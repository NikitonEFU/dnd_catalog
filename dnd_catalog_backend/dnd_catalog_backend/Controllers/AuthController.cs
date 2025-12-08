// Controllers/AuthController.cs

using Microsoft.AspNetCore.Mvc;
using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;

namespace dnd_catalog_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            try
            {
                var user = await _authService.Register(userDto.Username, userDto.Password);

                // Після успішної реєстрації одразу видаємо токен
                var token = _authService.GenerateJwtToken(user);
                return Ok(new { Message = "Registration successful", Token = token, Role = user.Role });
            }
            catch (InvalidOperationException ex)
            {
                // Якщо користувач вже існує
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            var user = await _authService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Message = "Login successful", Token = token, Role = user.Role });
        }
    }
}