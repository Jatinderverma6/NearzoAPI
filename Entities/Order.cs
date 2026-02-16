namespace NearzoAPI.Entities
{
    public enum OrderStatus
    {
        Pending,
        Accepted,
        Preparing,
        Ready,
        Completed,
        Cancelled
    }
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentUTR { get; set; }
        public bool IsPaymentConfirmed { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
