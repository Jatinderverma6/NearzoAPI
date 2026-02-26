using NearzoAPI.DTOs.Auth;
using NearzoAPI.Entities;

namespace NearzoAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto?> LoginAsync(LoginDto dto);   // ✅ nullable

        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

        Task<List<User>> GetUsers();

        Task<string> ForgotPasswordAsync(string email);

        Task<string?> VerifyResetOtpAsync(string email, string otp);

        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
