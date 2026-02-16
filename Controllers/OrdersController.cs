//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NearzoAPI.DTOs.Order;
//using NearzoAPI.Entities;
//using NearzoAPI.Services.Interfaces;
//using System.Security.Claims;

//namespace NearzoAPI.Controllers
//{
//    [ApiController]
//    [Route("api/orders")]
//    public class OrdersController : ControllerBase
//    {
//        private readonly IOrderService _orderService;
//        public OrdersController(IOrderService orderService)
//        {
//            _orderService = orderService;
//        }

//        [Authorize(Roles = "User")]
//        [HttpPost]
//        public async Task<IActionResult> Create(CreateOrderDto dto)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            return Ok(await _orderService.CreateAsync(dto, userId));
//        }

//        [Authorize(Roles = "User")]
//        [HttpGet("my")]
//        public async Task<IActionResult> MyOrders()
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            return Ok(await _orderService.GetUserOrdersAsync(userId));
//        }

//        [Authorize(Roles = "ShopProvider")]
//        [HttpPut("{id}/status")]
//        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] OrderStatus status)
//            => Ok(await _orderService.UpdateStatusAsync(id, status));
//    }
//}
