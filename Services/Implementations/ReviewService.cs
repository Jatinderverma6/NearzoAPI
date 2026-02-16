//using NearzoAPI.Data;
//using NearzoAPI.DTOs.Review;
//using NearzoAPI.Entities;
//using NearzoAPI.Services.Interfaces;

//namespace NearzoAPI.Services.Implementations
//{
//    public class ReviewService : IReviewService
//    {
//        private readonly ApplicationDbContext _context;

//        public ReviewService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<ReviewDto> AddReviewAsync(CreateReviewDto dto, int userId)
//        {
//            var review = new Review
//            {
//                UserId = userId,
//                ShopId = dto.ShopId,
//                Rating = dto.Rating,
//                Comment = dto.Comment,
//                CreatedAt = DateTime.UtcNow,
//                IsFlagged = false // AI logic can update this later
//            };

//            _context.Reviews.Add(review);
//            await _context.SaveChangesAsync();

//            return new ReviewDto
//            {
//                Id = review.Id,
//                Rating = review.Rating,
//                Comment = review.Comment
//            };
//        }

//        public async Task<List<ReviewDto>> GetShopReviewsAsync(int shopId)
//        {
//            return await _context.Reviews
//                .Where(r => r.ShopId == shopId)
//                .Select(r => new ReviewDto
//                {
//                    Id = r.Id,
//                    Rating = r.Rating,
//                    Comment = r.Comment
//                })
//                .ToListAsync();
//        }

//        public async Task<bool> FlagReviewAsync(int reviewId)
//        {
//            var review = await _context.Reviews.FindAsync(reviewId);

//            if (review == null)
//                return false;

//            review.IsFlagged = true;
//            await _context.SaveChangesAsync();

//            return true;
//        }
//    }
//}
