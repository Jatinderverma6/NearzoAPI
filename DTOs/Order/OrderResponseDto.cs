using NearzoAPI.Entities;

namespace NearzoAPI.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaymentConfirmed { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
