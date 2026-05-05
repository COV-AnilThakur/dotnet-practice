using Ecommerce_DBFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync(int? categoryId, string? searchTerm, string? sortOrder);
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Category>> GetCategoriesForDropdownAsync();
        Task<Product?> GetProductWithCategoryByIdAsync(int id);
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
    }

    public class ProductService : IProductService
    {
        private readonly EcommerceDbFirstContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(EcommerceDbFirstContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync(int? categoryId, string? searchTerm, string? sortOrder)
        {
            _logger.LogInformation(
                "Fetching products. CategoryId: {CategoryId}, SearchTerm: {SearchTerm}, SortOrder: {SortOrder}",
                categoryId, searchTerm, sortOrder);

            var productQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryId);
            }

            productQuery = sortOrder switch
            {
                "price_desc" => productQuery.OrderByDescending(p => p.Price),
                "price_asc" => productQuery.OrderBy(p => p.Price),
                _ => productQuery.OrderBy(p => p.Name)
            };

            return await productQuery.ToListAsync();
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _context.Categories.ToListAsync();
        }

        public Task<List<Category>> GetCategoriesForDropdownAsync()
        {
            return _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        }

        public Task<Product?> GetProductWithCategoryByIdAsync(int id)
        {
            return _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            return _context.Products.FindAsync(id).AsTask();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product created successfully. ProductId: {ProductId}", product.ProductId);
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product updated successfully. ProductId: {ProductId}", product.ProductId);
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Product deleted. ProductId: {ProductId}", product.ProductId);
        }

        public Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
