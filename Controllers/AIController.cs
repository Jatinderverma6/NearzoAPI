using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NearzoAPI.Common;
using NearzoAPI.DTOs.Auth;
using NearzoAPI.Services.Interfaces;
using Org.BouncyCastle.Asn1.Crmf;

namespace NearzoAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController(IAIService aiService) : ControllerBase
    {
        private readonly IAIService _aiService = aiService;

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string request)
        {
            if (request.IsNullOrEmpty())
                return BadRequest(request);
            //
            try
            {
                var response = await _aiService.GetChatResponseAsync(request);
                return Ok(ApiResponse<string>.SuccessResponse(response));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(
                 ApiResponse<object>.FailResponse($"Error: {ex.Message}")
             );
            }
        }

        [HttpPost("openrouterchat")]
        public async Task<IActionResult> openrouterchat([FromBody] string request)
        {
            if (request.IsNullOrEmpty())
                return BadRequest(request);
            //
            try
            {
                var response = await _aiService.GetOpenRouterChatResponseAsync(request);
                return Ok(ApiResponse<string>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(
                 ApiResponse<object>.FailResponse($"Error: {ex.Message}")
             );
            }
        }
    }
}
