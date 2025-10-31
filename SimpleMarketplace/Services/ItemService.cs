using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Data;
using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                .Include(i => i.Seller)
                .Include(i => i.InterestedBuyers)
                .OrderByDescending(i => i.DatePosted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetAvailableItemsAsync()
        {
            return await _context.Items
                .Include(i => i.Seller)
                .Include(i => i.Category)
                .Include(i => i.InterestedBuyers)
                .Where(i => !i.IsSold)
                .OrderByDescending(i => i.DatePosted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> SearchItemsAsync(string? searchTerm, int? categoryId)
        {
            var query = _context.Items
                .Include(i => i.Seller)
                .Include(i => i.Category)
                .Include(i => i.InterestedBuyers)
                .Where(i => !i.IsSold);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i => i.Title.Contains(searchTerm) || i.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            return await query
                .OrderByDescending(i => i.DatePosted)
                .ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items
                .Include(i => i.Seller)
                .Include(i => i.Category)
                .Include(i => i.InterestedBuyers)
                    .ThenInclude(interest => interest.Buyer)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Item>> GetItemsBySellerAsync(string sellerId)
        {
            return await _context.Items
                .Include(i => i.InterestedBuyers)
                .Where(i => i.SellerId == sellerId)
                .OrderByDescending(i => i.DatePosted)
                .ToListAsync();
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            item.DatePosted = DateTime.UtcNow;
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            _context.Items.Update(item);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> MarkItemAsSoldAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            item.IsSold = true;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Interest>> GetItemInterestsAsync(int itemId)
        {
            return await _context.Interests
                .Include(i => i.Buyer)
                .Where(i => i.ItemId == itemId)
                .OrderByDescending(i => i.DateMarked)
                .ToListAsync();
        }
    }
}
