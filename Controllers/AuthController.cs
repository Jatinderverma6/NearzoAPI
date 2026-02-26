using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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

        [EnableRateLimiting("EmailPolicy")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(
                        ApiResponse<object>.FailResponse("Email is required")
                    );
                }

                var result = await _authService.ForgotPasswordAsync(request.Email);

                return Ok(
                    ApiResponse<object>.SuccessResponse(new
                    {
                        msg = result
                    })
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.FailResponse($"Something went wrong: {ex.Message}")
                );
            }
        }

        // ==============================
        // VERIFY OTP (Screen 2)
        // ==============================
        [HttpPost("verify-reset-otp")]
        public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Otp))
            {
                return BadRequest(
                    ApiResponse<object>.FailResponse("Email and OTP are required")
                );
            }

            var token = await _authService.VerifyResetOtpAsync(dto.Email, dto.Otp);

            if (token == null)
                return BadRequest(
                    ApiResponse<object>.FailResponse("Invalid or expired OTP")
                );

            return Ok(
                ApiResponse<object>.SuccessResponse(new
                {
                    resetToken = token
                })
            );
        }

        // ==============================
        // RESET PASSWORD (Screen 3)
        // ==============================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Token) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(
                    ApiResponse<object>.FailResponse("All fields are required")
                );
            }

            var result = await _authService.ResetPasswordAsync(
                dto.Email,
                dto.Token,
                dto.NewPassword
            );

            if (!result)
                return BadRequest(
                    ApiResponse<object>.FailResponse("Invalid or expired reset token")
                );

            return Ok(
                ApiResponse<object>.SuccessResponse(new
                {
                    msg = "Password reset successful"
                })
            );
        }

    }
}
