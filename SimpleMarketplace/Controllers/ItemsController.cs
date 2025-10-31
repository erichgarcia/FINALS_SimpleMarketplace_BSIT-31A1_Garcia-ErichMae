using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleMarketplace.Models;
using SimpleMarketplace.Services;
using SimpleMarketplace.ViewModels;

namespace SimpleMarketplace.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IInterestService _interestService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ItemsController(
            IItemService itemService,
            IInterestService interestService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _itemService = itemService;
            _interestService = interestService;
            _categoryService = categoryService;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Items
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId)
        {
            IEnumerable<Item> items;
            
            if (!string.IsNullOrWhiteSpace(searchTerm) || categoryId.HasValue)
            {
                items = await _itemService.SearchItemsAsync(searchTerm, categoryId);
            }
            else
            {
                items = await _itemService.GetAvailableItemsAsync();
            }
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.UserId = userId;
            }
            
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedCategoryId = categoryId;
            
            return View(items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.HasMarkedInterest = await _interestService.HasUserMarkedInterestAsync(userId!, id);
                ViewBag.UserId = userId;
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                string? imageFileName = null;
                
                // Handle image upload
                if (model.Image != null && model.Image.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                    
                    if (allowedExtensions.Contains(extension))
                    {
                        imageFileName = $"{Guid.NewGuid()}{extension}";
                        var imagePath = Path.Combine(_environment.WebRootPath, "images", "items", imageFileName);
                        
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(stream);
                        }
                    }
                }
                
                var item = new Item
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ImageFileName = imageFileName,
                    SellerId = userId!,
                    DatePosted = DateTime.UtcNow
                };

                await _itemService.CreateItemAsync(item);
                TempData["Success"] = "Item posted successfully!";
                return RedirectToAction(nameof(MyItems));
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(model);
        }

        // GET: Items/MyItems
        [Authorize]
        public async Task<IActionResult> MyItems()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _itemService.GetItemsBySellerAsync(userId!);
            return View(items);
        }

        // GET: Items/MyInterests
        [Authorize]
        public async Task<IActionResult> MyInterests()
        {
            var userId = _userManager.GetUserId(User);
            var interests = await _interestService.GetUserInterestsAsync(userId!);
            return View(interests);
        }

        // POST: Items/MarkInterest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> MarkInterest(int id)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _itemService.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            // Prevent seller from marking interest in their own item
            if (item.SellerId == userId)
            {
                TempData["Error"] = "You cannot mark interest in your own item.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _interestService.MarkInterestAsync(userId!, id);
            
            if (result)
            {
                TempData["Success"] = "Interest marked successfully!";
            }
            else
            {
                TempData["Error"] = "You have already marked interest in this item.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Items/RemoveInterest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveInterest(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _interestService.RemoveInterestAsync(userId!, id);
            
            TempData["Success"] = "Interest removed.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Items/MarkAsSold/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> MarkAsSold(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            
            if (item == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            
            // Only the seller can mark the item as sold
            if (item.SellerId != userId)
            {
                return Forbid();
            }

            await _itemService.MarkItemAsSoldAsync(id);
            TempData["Success"] = "Item marked as sold!";
            
            return RedirectToAction(nameof(MyItems));
        }

        // POST: Items/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            
            if (item == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            
            // Only the seller can delete the item
            if (item.SellerId != userId)
            {
                return Forbid();
            }

            await _itemService.DeleteItemAsync(id);
            TempData["Success"] = "Item deleted successfully!";
            
            return RedirectToAction(nameof(MyItems));
        }
    }
}
