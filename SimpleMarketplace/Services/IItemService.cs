using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetAvailableItemsAsync();
        Task<Item?> GetItemByIdAsync(int id);
        Task<IEnumerable<Item>> GetItemsBySellerAsync(string sellerId);
        Task<Item> CreateItemAsync(Item item);
        Task<bool> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
        Task<bool> MarkItemAsSoldAsync(int id);
        Task<IEnumerable<Interest>> GetItemInterestsAsync(int itemId);
    }
}
