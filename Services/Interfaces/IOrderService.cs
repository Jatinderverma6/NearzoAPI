//using NearzoAPI.DTOs.Order;
//using NearzoAPI.Entities;

//namespace NearzoAPI.Services.Interfaces
//{
//    public interface IOrderService
//    {
//        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, string userId);
//        Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(string userId);
//        Task<IEnumerable<OrderResponseDto>> GetShopOrdersAsync(string ownerId);
//        Task<OrderResponseDto> UpdateStatusAsync(int orderId, OrderStatus status, string ownerId);
//        Task<bool> ConfirmPaymentAsync(int orderId, string ownerId);
//    }
//}
