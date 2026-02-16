//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NearzoAPI.DTOs.Deal;
//using NearzoAPI.Services.Interfaces;

//namespace NearzoAPI.Controllers
//{
//    [ApiController]
//    [Route("api/deals")]
//    public class DealsController : ControllerBase
//    {
//        private readonly IDealService _dealService;

//        public DealsController(IDealService dealService)
//        {
//            _dealService = dealService;
//        }

//        [HttpGet("shop/{shopId}")]
//        public async Task<IActionResult> GetByShop(int shopId)
//            => Ok(await _dealService.GetByShopAsync(shopId));

//        [Authorize(Roles = "ShopProvider")]
//        [HttpPost]
//        public async Task<IActionResult> Create(CreateDealDto dto)
//            => Ok(await _dealService.CreateAsync(dto));
//    }
//}
