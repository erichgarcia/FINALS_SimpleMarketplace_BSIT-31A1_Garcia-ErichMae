using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Data;
using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public class InterestService : IInterestService
    {
        private readonly ApplicationDbContext _context;

        public InterestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> MarkInterestAsync(string buyerId, int itemId)
        {
            // Check if interest already exists
            var existingInterest = await _context.Interests
                .FirstOrDefaultAsync(i => i.BuyerId == buyerId && i.ItemId == itemId);

            if (existingInterest != null)
                return false; // Already marked

            var interest = new Interest
            {
                BuyerId = buyerId,
                ItemId = itemId,
                DateMarked = DateTime.UtcNow
            };

            _context.Interests.Add(interest);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RemoveInterestAsync(string buyerId, int itemId)
        {
            var interest = await _context.Interests
                .FirstOrDefaultAsync(i => i.BuyerId == buyerId && i.ItemId == itemId);

            if (interest == null)
                return false;

            _context.Interests.Remove(interest);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> HasUserMarkedInterestAsync(string buyerId, int itemId)
        {
            return await _context.Interests
                .AnyAsync(i => i.BuyerId == buyerId && i.ItemId == itemId);
        }

        public async Task<IEnumerable<Interest>> GetUserInterestsAsync(string userId)
        {
            return await _context.Interests
                .Include(i => i.Item)
                    .ThenInclude(item => item.Seller)
                .Where(i => i.BuyerId == userId)
                .OrderByDescending(i => i.DateMarked)
                .ToListAsync();
        }
    }
}
