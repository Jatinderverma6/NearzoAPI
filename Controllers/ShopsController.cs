//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NearzoAPI.DTOs.Shop;
//using NearzoAPI.Services.Interfaces;
//using System.Security.Claims;

//namespace NearzoAPI.Controllers
//{
//    [ApiController]
//    [Route("api/shops")]
//    public class ShopsController : ControllerBase
//    {
//        private readonly IShopService _shopService;

//        public ShopsController(IShopService shopService)
//        {
//            _shopService = shopService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//            => Ok(await _shopService.GetAllAsync());

//        [HttpGet("{id}")]
//        public async Task<IActionResult> Get(int id)
//            => Ok(await _shopService.GetByIdAsync(id));

//        [Authorize(Roles = "ShopProvider")]
//        [HttpPost]
//        public async Task<IActionResult> Create(CreateShopDto dto)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            return Ok(await _shopService.CreateAsync(dto, userId));
//        }
//    }
//}
