namespace NearzoAPI.DTOs.Deal
{
    public class CreateDealDto
    {
        public int ShopId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
