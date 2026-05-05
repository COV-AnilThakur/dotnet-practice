using Ecommerce_DBFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }

    public class CategoryService : ICategoryService
    {
        private readonly EcommerceDbFirstContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(EcommerceDbFirstContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching categories list.");
            return _context.Categories.ToListAsync();
        }

        public Task<Category?> GetCategoryByIdAsync(int id)
        {
            return _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category created successfully. CategoryId: {CategoryId}", category.CategoryId);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category updated successfully. CategoryId: {CategoryId}", category.CategoryId);
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Category deleted. CategoryId: {CategoryId}", category.CategoryId);
        }
    }
}
