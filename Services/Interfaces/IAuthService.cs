using NearzoAPI.DTOs.Auth;
using NearzoAPI.Entities;

namespace NearzoAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

        Task<List<User>> GetUsers();
    }
}
