namespace NearzoAPI.DTOs.Review
{
    public class CreateReviewDto
    {
        public int ShopId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
