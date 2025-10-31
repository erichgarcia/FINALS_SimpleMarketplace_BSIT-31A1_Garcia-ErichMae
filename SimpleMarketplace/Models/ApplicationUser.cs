using Microsoft.AspNetCore.Identity;

namespace SimpleMarketplace.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        
        // Navigation properties
        public virtual ICollection<Item> ItemsPosted { get; set; } = new List<Item>();
        public virtual ICollection<Interest> Interests { get; set; } = new List<Interest>();
    }
}
