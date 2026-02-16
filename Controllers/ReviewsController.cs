//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NearzoAPI.DTOs.Review;
//using NearzoAPI.Services.Interfaces;
//using System.Security.Claims;

//namespace NearzoAPI.Controllers
//{
//    [ApiController]
//    [Route("api/reviews")]
//    public class ReviewsController : ControllerBase
//    {
//        private readonly IReviewService _reviewService;

//        public ReviewsController(IReviewService reviewService)
//        {
//            _reviewService = reviewService;
//        }

//        [Authorize(Roles = "User")]
//        [HttpPost]
//        public async Task<IActionResult> Create(CreateReviewDto dto)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            return Ok(await _reviewService.CreateAsync(dto, userId));
//        }

//        [Authorize(Roles = "ShopProvider")]
//        [HttpPost("{id}/reply")]
//        public async Task<IActionResult> Reply(int id, [FromBody] string reply)
//            => Ok(await _reviewService.ReplyAsync(id, reply));
//    }
//}
