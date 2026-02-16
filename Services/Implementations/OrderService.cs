//using NearzoAPI.DTOs.Order;
//using NearzoAPI.Entities;
//using NearzoAPI.Services.Interfaces;

//namespace NearzoAPI.Services.Implementations
//{
//    public class OrderService : IOrderService
//    {
//        private readonly AppDbContext _context;

//        public OrderService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, int userId)
//        {
//            var order = new Order
//            {
//                UserId = userId,
//                ShopId = dto.ShopId,
//                TotalAmount = dto.TotalAmount,
//                Status = "Pending",
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync();

//            return new OrderDto
//            {
//                Id = order.Id,
//                Status = order.Status,
//                TotalAmount = order.TotalAmount
//            };
//        }

//        public async Task<List<OrderDto>> GetShopOrdersAsync(int shopId)
//        {
//            return await _context.Orders
//                .Where(o => o.ShopId == shopId)
//                .Select(o => new OrderDto
//                {
//                    Id = o.Id,
//                    Status = o.Status,
//                    TotalAmount = o.TotalAmount
//                })
//                .ToListAsync();
//        }

//        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, int shopId)
//        {
//            var order = await _context.Orders
//                .FirstOrDefaultAsync(o => o.Id == orderId && o.ShopId == shopId);

//            if (order == null)
//                return false;

//            order.Status = status;
//            await _context.SaveChangesAsync();

//            return true;
//        }
//    }
//}
