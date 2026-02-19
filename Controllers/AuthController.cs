using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NearzoAPI.Common;
using NearzoAPI.DTOs.Auth;
using NearzoAPI.Services.Interfaces;

namespace NearzoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(
                        ApiResponse<AuthResponseDto>.FailResponse(ex.Message)
                    );
            }
            catch (Exception e)
            {
                return StatusCode(500,
                    ApiResponse<AuthResponseDto>.FailResponse(e.Message)
                );
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(
            ApiResponse<AuthResponseDto>.FailResponse("Invalid credentials")
        );

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }
        [HttpGet("Users")]
        public async Task<IActionResult> getAllUsers()
        {
            var result = await _authService.GetUsers();
            return Ok(result);
        }
    }
}
