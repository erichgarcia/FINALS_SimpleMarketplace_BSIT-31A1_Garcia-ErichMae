using System.ComponentModel.DataAnnotations;

namespace SimpleMarketplace.Models
{
    public class Interest
    {
        public int Id { get; set; }
        
        // Foreign keys
        [Required]
        public string BuyerId { get; set; } = string.Empty;
        
        [Required]
        public int ItemId { get; set; }
        
        public DateTime DateMarked { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ApplicationUser Buyer { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
    }
}
