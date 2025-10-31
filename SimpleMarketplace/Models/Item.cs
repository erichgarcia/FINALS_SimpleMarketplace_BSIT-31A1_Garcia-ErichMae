using System.ComponentModel.DataAnnotations;

namespace SimpleMarketplace.Models
{
    public class Item
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
        
        public bool IsSold { get; set; } = false;
        
        [StringLength(255)]
        public string? ImageFileName { get; set; }
        
        // Foreign keys
        [Required]
        public string SellerId { get; set; } = string.Empty;
        
        public int? CategoryId { get; set; }
        
        // Navigation properties
        public virtual ApplicationUser Seller { get; set; } = null!;
        public virtual Category? Category { get; set; }
        public virtual ICollection<Interest> InterestedBuyers { get; set; } = new List<Interest>();
    }
}
