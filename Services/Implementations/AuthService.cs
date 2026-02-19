using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NearzoAPI.DTOs.Auth;
using NearzoAPI.Entities;
using NearzoAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // ==============================
    // REGISTER
    // ==============================
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            UserName = dto.Email, // IMPORTANT
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role ?? "User",
            CreatedAt = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);

        return await GenerateAuthResponse(user);
    }

    // ==============================
    // LOGIN
    // ==============================
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return null;

        return await GenerateAuthResponse(user);
    }

    // ==============================
    // REFRESH TOKEN
    // ==============================
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = _userManager.Users
            .FirstOrDefault(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new Exception("Invalid or expired refresh token");

        return await GenerateAuthResponse(user);
    }

    // ==============================
    // TOKEN GENERATION
    // ==============================
    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        var expiryDays = int.TryParse(
            _configuration["Jwt:RefreshTokenExpiryDays"],
            out var days) ? days : 7;

        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email!,
            Role = user.Role,
            FullName = user.FullName,
            Expiration = user.RefreshTokenExpiryTime.Value
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(_configuration["Jwt:AccessTokenExpiryMinutes"])
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

    public async Task<List<User>> GetUsers()
    {
        return await _userManager.Users.ToListAsync();
    }
}
