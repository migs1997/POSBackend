using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;
using POSBackend.Services;
using System;
using System.Threading.Tasks;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(IMongoClient mongoClient, IPasswordHasher passwordHasher)
        {
            var database = mongoClient.GetDatabase("mobapp");
            _users = database.GetCollection<User>("users");
            _passwordHasher = passwordHasher;
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("✅ Auth API is running");

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "Invalid registration data." });

            var existingUser = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (existingUser != null)
                return Conflict(new { message = "Email already registered." });

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                City = dto.City,
                Barangay = dto.Barangay,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _users.InsertOneAsync(user);
            return Ok(new { message = "Registration successful!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            bool isPasswordValid = _passwordHasher.VerifyPassword(dto.Password, user.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new
            {
                message = "Login successful!",
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.City,
                    user.Barangay
                }
            });
        }
    }

    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
