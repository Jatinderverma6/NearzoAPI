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
using static System.Net.WebRequestMethods;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
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

    public async Task<string> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // 🔐 Security: Do NOT reveal if user exists
        if (user == null)
        {
            return $"Verification code has been sent to email {email}";
        }

        string otp = await GenerateAndGetOtpAsync(email);
        if(otp != null)
        {
            string emailBody = $@"
            <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);"">
                <div style=""background-color: #0078d4; padding: 30px; text-align: center;"">
                    <h1 style=""color: white; margin: 0; font-size: 24px;"">Nearzo App</h1>
                </div>
                <div style=""padding: 40px; background-color: #ffffff; color: #333333; line-height: 1.6;"">
                    <h2 style=""color: #0078d4; margin-top: 0;"">Reset Your Password</h2>
                    <p>Dear Nearzo App User,</p>
                    <p>We received a request to reset your password. Use the verification code below to proceed. This code will expire shortly.</p>

                    <div style=""margin: 30px 0; text-align: center;"">
                        <div style=""display: inline-block; background-color: #f3f2f1; padding: 15px 30px; border-radius: 8px; border: 2px dashed #0078d4;"">
                            <span style=""font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #0078d4;"" href=""nearzo://verify?code={otp}"">{otp}</span>
                        </div>
                    </div>

                    <p style=""font-size: 14px; color: #666666;"">If you didn't request this, you can safely ignore this email.</p>
                </div>
                <div style=""background-color: #f9f9f9; padding: 20px; text-align: center; font-size: 12px; color: #999999;"">
                    &copy; {DateTime.Now.Year} Nearzo App. All rights reserved.
                </div>
            </div>";

            // Send OTP email
            await _emailService.SendEmailAsync(email, "Reset Your Password - Nearzo App", emailBody);

        }
        return $"Verification code has been sent to email {email}";
    }

    private async Task<string> GenerateAndGetOtpAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // Prevent enumeration attack
        if (user == null)
            return "";

        var otp = new Random().Next(100000, 999999).ToString();

        user.PasswordResetOtpHash = BCrypt.Net.BCrypt.HashPassword(otp);
        user.PasswordResetOtpExpiry = DateTime.UtcNow.AddMinutes(5);

        await _userManager.UpdateAsync(user);

        return otp;
    }

    public async Task<string?> VerifyResetOtpAsync(string email, string otp)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null ||
            user.PasswordResetOtpExpiry < DateTime.UtcNow ||
            !BCrypt.Net.BCrypt.Verify(otp, user.PasswordResetOtpHash))
        {
            return null;
        }

        // Generate short-lived secure reset token
        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.TempResetToken = resetToken;
        user.TempResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        await _userManager.UpdateAsync(user);

        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null ||
            user.TempResetToken != token ||
            user.TempResetTokenExpiry < DateTime.UtcNow)
            return false;

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!result.Succeeded)
            return false;

        // Clear temp tokens
        user.TempResetToken = null;
        user.TempResetTokenExpiry = null;
        user.PasswordResetOtpHash = null;
        user.PasswordResetOtpExpiry = null;

        await _userManager.UpdateAsync(user);

        return true;
    }
}
