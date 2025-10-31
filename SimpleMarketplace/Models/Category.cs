using System.ComponentModel.DataAnnotations;

namespace SimpleMarketplace.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        // Navigation property
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
