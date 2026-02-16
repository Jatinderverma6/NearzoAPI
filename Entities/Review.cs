namespace NearzoAPI.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShopId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Reply { get; set; }
        public bool IsFlagged { get; set; } // For AI fake detection
        public DateTime CreatedAt { get; set; }
    }
}
