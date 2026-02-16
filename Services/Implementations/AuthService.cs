using Microsoft.IdentityModel.Tokens;
using NearzoAPI.Data;
using NearzoAPI.DTOs.Auth;
using NearzoAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NearzoAPI.Entities;

namespace NearzoAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ==============================
        // REGISTER
        // ==============================
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Email already exists"); // 409 Conflict

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateAuthResponse(user);
        }

        // ==============================
        // LOGIN
        // ==============================
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    return null;

                return await GenerateAuthResponse(user);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Login Error: {e}");
                return null;
            }
        }

        // ==============================
        // REFRESH TOKEN
        // ==============================
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            //var user = await _context.Users
            //    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            //if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            //    throw new Exception("Invalid or expired refresh token");

            //return await GenerateAuthResponse(user);
            return new AuthResponseDto();
        }

        // ==============================
        // TOKEN GENERATION
        // ==============================
        private async Task<AuthResponseDto> GenerateAuthResponse(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiryDays"])
            );

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                Expiration = user.RefreshTokenExpiryTime ?? DateTime.UtcNow
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyValue = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(keyValue))
                throw new Exception("JWT Key is missing in configuration.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue)
            );


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(jwtSettings["AccessTokenExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        async Task<List<User>> IAuthService.GetUsers()
        {
            try
            {
                if (_context != null)
                {
                    var users = await _context.Users.ToListAsync();
                    return users;
                }
                else
                {
                    return [];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return [];
            }
        }
    }

}
