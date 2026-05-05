using AutoMapper;
using Ecommerce_DBFirst.Models;
using Ecommerce_DBFirst.Services;
using Ecommerce_DBFirst.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IInventoryApiClient _inventoryApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            IInventoryApiClient inventoryApiClient,
            IMapper mapper,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _inventoryApiClient = inventoryApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("")]
        [Route("list")]
        public async Task<IActionResult> Index(int? categoryId, string? searchTerm, string? sortOrder)
        {
            _logger.LogInformation("Loading products page.");

            ViewBag.Title = "Product Management";
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentCategory"] = categoryId;

            var products = await _productService.GetProductsAsync(categoryId, searchTerm, sortOrder);
            var categories = await _productService.GetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", categoryId);

            var productVms = _mapper.Map<List<ProductVM>>(products);
            await ApplyInventoryStockAsync(productVms);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductTable", productVms);
            }

            return View(productVms);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("add")]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropdown();
            return View(new ProductFormVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM productVm)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating product.");
                TempData["ErrorMessage"] = "Error: Product already exists or invalid data.";
                await PopulateCategoriesDropdown(productVm.CategoryId);
                return View(productVm);
            }

            var product = _mapper.Map<Product>(productVm);
            await _productService.AddProductAsync(product);
            TempData["SuccessMessage"] = "Product added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductWithCategoryByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product details requested for invalid ProductId: {ProductId}", id);
                TempData["ErrorMessage"] = "Product details could not be found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Title = "Product Details - " + product.Name;
            var productVm = _mapper.Map<ProductVM>(product);
            return View(productVm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit product called with null id.");
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                _logger.LogWarning("Edit product requested for missing ProductId: {ProductId}", id.Value);
                return NotFound();
            }

            await PopulateCategoriesDropdown(product.CategoryId);
            var productVm = _mapper.Map<ProductFormVM>(product);
            return View(productVm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormVM productVm)
        {
            if (id != productVm.ProductId)
            {
                _logger.LogWarning("Edit product id mismatch. RouteId: {RouteId}, ModelId: {ModelId}", id, productVm.ProductId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when editing ProductId: {ProductId}", id);
                await PopulateCategoriesDropdown(productVm.CategoryId);
                return View(productVm);
            }

            try
            {
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Edit failed. Product not found: {ProductId}", id);
                    return NotFound();
                }

                _mapper.Map(productVm, existingProduct);
                await _productService.UpdateProductAsync(existingProduct);
                TempData["SuccessMessage"] = "Product updated successfully!";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating ProductId: {ProductId}", id);
                TempData["ErrorMessage"] = "Error updating product.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete product called with null id.");
                return NotFound();
            }

            var product = await _productService.GetProductWithCategoryByIdAsync(id.Value);
            if (product == null)
            {
                _logger.LogWarning("Delete product requested for missing ProductId: {ProductId}", id.Value);
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Delete failed. Product not found: {ProductId}", id);
                return RedirectToAction(nameof(Index));
            }

            await _productService.DeleteProductAsync(product);
            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("~/categories/{categoryId:int}/products")]
        public async Task<IActionResult> GetByCategories(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            ViewBag.Title = "Products in Category";
            var productVms = _mapper.Map<List<ProductVM>>(products);
            await ApplyInventoryStockAsync(productVms);
            return View("Index", productVms);
        }

        private async Task ApplyInventoryStockAsync(List<ProductVM> productVms)
        {
            if (productVms.Count == 0)
            {
                return;
            }

            var inventoryItems = await _inventoryApiClient.GetAllAsync();
            if (inventoryItems.Count == 0)
            {
                _logger.LogWarning("No inventory data available while loading products list.");
                foreach (var vm in productVms)
                {
                    vm.StockQuantity = null;
                }

                return;
            }

            var stockByProductId = inventoryItems
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.First().StockQuantity);

            foreach (var vm in productVms)
            {
                vm.StockQuantity = stockByProductId.TryGetValue(vm.ProductId, out var quantity) ? quantity : null;
            }
        }

        private async Task PopulateCategoriesDropdown(int? selectedCategory = null)
        {
            var categories = await _productService.GetCategoriesForDropdownAsync();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", selectedCategory);
        }
    }
}
