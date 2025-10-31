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
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemsController(
            IItemService itemService,
            IInterestService interestService,
            UserManager<ApplicationUser> userManager)
        {
            _itemService = itemService;
            _interestService = interestService;
            _userManager = userManager;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetAvailableItemsAsync();
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.UserId = userId;
            }
            
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
        public IActionResult Create()
        {
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
                
                var item = new Item
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    SellerId = userId!,
                    DatePosted = DateTime.UtcNow
                };

                await _itemService.CreateItemAsync(item);
                return RedirectToAction(nameof(MyItems));
            }

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
