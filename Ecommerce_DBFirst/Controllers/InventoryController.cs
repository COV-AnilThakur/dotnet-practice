using Ecommerce_DBFirst.Services;
using Ecommerce_DBFirst.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    [Route("inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryApiClient _inventoryApiClient;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryApiClient inventoryApiClient, ILogger<InventoryController> logger)
        {
            _inventoryApiClient = inventoryApiClient;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var inventory = await _inventoryApiClient.GetAllAsync();
                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory list from API.");
                TempData["ErrorMessage"] = "Unable to load inventory data right now.";
                return View(new List<InventoryItemVM>());
            }
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> Details(int productId)
        {
            try
            {
                var item = await _inventoryApiClient.GetByProductIdAsync(productId);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Inventory record not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory details for ProductId: {ProductId}", productId);
                TempData["ErrorMessage"] = "Unable to load inventory details right now.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("add")]
        public IActionResult Create()
        {
            return View(new InventoryCreateVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _inventoryApiClient.CreateAsync(model);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Unable to create inventory record.";
                _logger.LogWarning("Inventory create failed in MVC. ProductId: {ProductId}, Error: {Error}", model.ProductId, result.Error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Inventory record created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("edit/{productId:int}")]
        public async Task<IActionResult> UpdateStock(int productId)
        {
            var item = await _inventoryApiClient.GetByProductIdAsync(productId);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory record not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new InventoryUpdateVM
            {
                ProductId = item.ProductId,
                StockQuantity = item.StockQuantity
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit/{productId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int productId, InventoryUpdateVM model)
        {
            if (productId != model.ProductId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _inventoryApiClient.UpdateStockAsync(productId, model);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Unable to update inventory stock.";
                _logger.LogWarning("Inventory update failed in MVC. ProductId: {ProductId}, Error: {Error}", productId, result.Error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Inventory stock updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{productId:int}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var item = await _inventoryApiClient.GetByProductIdAsync(productId);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory record not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{productId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productId)
        {
            var result = await _inventoryApiClient.DeleteAsync(productId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Unable to delete inventory record.";
                _logger.LogWarning("Inventory delete failed in MVC. ProductId: {ProductId}, Error: {Error}", productId, result.Error);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Inventory record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
