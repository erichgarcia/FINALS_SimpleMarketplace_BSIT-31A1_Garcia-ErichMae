using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Data;
using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task InitializeCategoriesAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Phones, laptops, gadgets" },
                    new Category { Name = "Furniture", Description = "Home and office furniture" },
                    new Category { Name = "Clothing", Description = "Clothes and accessories" },
                    new Category { Name = "Books", Description = "Books and magazines" },
                    new Category { Name = "Sports", Description = "Sports equipment" },
                    new Category { Name = "Other", Description = "Miscellaneous items" }
                };

                _context.Categories.AddRange(categories);
                await _context.SaveChangesAsync();
            }
        }
    }
}
