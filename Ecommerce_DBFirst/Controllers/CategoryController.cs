using Ecommerce_DBFirst.Models;
using Ecommerce_DBFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    [Route("categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Category Management";
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("add")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating category.");
                return View(category);
            }

            await _categoryService.AddCategoryAsync(category);
            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit category called with null id.");
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                _logger.LogWarning("Category not found for edit. CategoryId: {CategoryId}", id.Value);
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning("Edit category id mismatch. RouteId: {RouteId}, ModelId: {ModelId}", id, category.CategoryId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when updating CategoryId: {CategoryId}", id);
                return View(category);
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(category);
                TempData["SuccessMessage"] = "Category updated successfully!";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating CategoryId: {CategoryId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete category called with null id.");
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                _logger.LogWarning("Delete category requested for missing CategoryId: {CategoryId}", id.Value);
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Delete category failed. Missing CategoryId: {CategoryId}", id);
                return NotFound();
            }

            try
            {
                await _categoryService.DeleteCategoryAsync(category);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Delete category blocked by FK constraint. CategoryId: {CategoryId}", id);
                TempData["ErrorMessage"] = "Cannot delete category: It contains active products. Delete the products first.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
