namespace NearzoAPI.DTOs.Order
{
    public class CreateOrderDto
    {
        public int ShopId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentUTR { get; set; }
    }
}
