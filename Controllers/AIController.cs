using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            var response = await _aiService.GetChatResponseAsync(request);
            return Ok(new { response });
        }
    }
}
