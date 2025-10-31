using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public interface IInterestService
    {
        Task<bool> MarkInterestAsync(string buyerId, int itemId);
        Task<bool> RemoveInterestAsync(string buyerId, int itemId);
        Task<bool> HasUserMarkedInterestAsync(string buyerId, int itemId);
        Task<IEnumerable<Interest>> GetUserInterestsAsync(string userId);
    }
}
