namespace NearzoAPI.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Reply { get; set; }
        public bool IsFlagged { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
