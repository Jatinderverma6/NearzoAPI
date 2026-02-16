namespace NearzoAPI.Entities
{
    public class Deal
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsTodayDeal { get; set; }
    }
}
