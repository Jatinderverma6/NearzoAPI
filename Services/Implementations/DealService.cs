//using Microsoft.EntityFrameworkCore;
//using NearzoAPI.Data;
//using NearzoAPI.DTOs.Deal;
//using NearzoAPI.Entities;
//using NearzoAPI.Services.Interfaces;

//namespace NearzoAPI.Services.Implementations
//{
//    public class DealService : IDealService
//    {
//        private readonly ApplicationDbContext _context;

//        public DealService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ==============================
//        // GET TODAY DEALS
//        // ==============================
//        public async Task<List<DealDto>> GetTodayDealsAsync()
//        {
//            var today = DateTime.UtcNow.Date;

//            return await _context.Deals
//                .AsNoTracking()
//                .Where(d => d.StartDate.Date <= today && d.EndDate.Date >= today)
//                .Select(d => new DealDto
//                {
//                    Id = d.Id,
//                    Title = d.Title,
//                    Description = d.Description,
//                    DiscountPercentage = d.DiscountPercentage,
//                    ShopId = d.ShopId
//                })
//                .ToListAsync();
//        }

//        // ==============================
//        // CREATE DEAL
//        // ==============================
//        public async Task<DealDto> CreateDealAsync(CreateDealDto dto, int shopId)
//        {
//            if (dto.StartDate > dto.EndDate)
//                throw new ArgumentException("Start date cannot be greater than end date");

//            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
//                throw new ArgumentException("Discount must be between 1 and 100");

//            var deal = new Deal
//            {
//                Title = dto.Title.Trim(),
//                Description = dto.Description?.Trim(),
//                DiscountPercentage = dto.DiscountPercentage,
//                StartDate = dto.StartDate.ToUniversalTime(),
//                EndDate = dto.EndDate.ToUniversalTime(),
//                ShopId = shopId,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _context.Deals.AddAsync(deal);
//            await _context.SaveChangesAsync();

//            return new DealDto
//            {
//                Id = deal.Id,
//                Title = deal.Title,
//                Description = deal.Description,
//                DiscountPercentage = deal.DiscountPercentage,
//                ShopId = deal.ShopId
//            };
//        }

//        // ==============================
//        // DELETE DEAL
//        // ==============================
//        public async Task<bool> DeleteDealAsync(int dealId, int shopId)
//        {
//            var deal = await _context.Deals
//                .FirstOrDefaultAsync(d => d.Id == dealId && d.ShopId == shopId);

//            if (deal == null)
//                return false;

//            _context.Deals.Remove(deal);
//            await _context.SaveChangesAsync();

//            return true;
//        }
//    }
//}
