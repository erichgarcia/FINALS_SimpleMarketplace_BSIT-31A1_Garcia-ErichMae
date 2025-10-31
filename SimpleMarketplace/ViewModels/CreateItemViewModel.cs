using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SimpleMarketplace.ViewModels
{
    public class CreateItemViewModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Item Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        
        [Display(Name = "Item Image")]
        public IFormFile? Image { get; set; }
    }
}
