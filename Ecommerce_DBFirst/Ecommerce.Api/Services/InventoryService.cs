using Ecommerce.Api.Data;
using Ecommerce.Api.DTOs;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services
{
    public interface IInventoryService
    {
        Task<List<InventoryDto>> GetAllAsync();
        Task<InventoryDto?> GetByProductIdAsync(int productId);
        Task<(bool IsSuccess, string? Error, InventoryDto? Data)> CreateAsync(CreateInventoryDto input);
        Task<(bool IsSuccess, string? Error)> UpdateStockAsync(int productId, UpdateInventoryStockDto input);
        Task<(bool IsSuccess, string? Error)> DeleteByProductIdAsync(int productId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly EcommerceApiContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(EcommerceApiContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<InventoryDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all inventory records.");

            return await _context.Inventory
                .Include(i => i.Product)
                .Select(i => new InventoryDto
                {
                    InventoryId = i.InventoryId,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    StockQuantity = i.StockQuantity,
                    LastUpdated = i.LastUpdated
                })
                .ToListAsync();
        }

        public async Task<InventoryDto?> GetByProductIdAsync(int productId)
        {
            var inventory = await _context.Inventory
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory == null)
            {
                _logger.LogWarning("Inventory not found for ProductId: {ProductId}", productId);
                return null;
            }

            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                ProductName = inventory.Product?.Name ?? string.Empty,
                StockQuantity = inventory.StockQuantity,
                LastUpdated = inventory.LastUpdated
            };
        }

        public async Task<(bool IsSuccess, string? Error, InventoryDto? Data)> CreateAsync(CreateInventoryDto input)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == input.ProductId);
            if (!productExists)
            {
                _logger.LogWarning("Inventory create failed. Product not found: {ProductId}", input.ProductId);
                return (false, "Product does not exist.", null);
            }

            var inventoryExists = await _context.Inventory.AnyAsync(i => i.ProductId == input.ProductId);
            if (inventoryExists)
            {
                _logger.LogWarning("Inventory create failed. Record already exists for ProductId: {ProductId}", input.ProductId);
                return (false, "Inventory record already exists for this product.", null);
            }

            var entity = new Inventory
            {
                ProductId = input.ProductId,
                StockQuantity = input.StockQuantity,
                LastUpdated = DateTime.UtcNow
            };

            _context.Inventory.Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Inventory created. ProductId: {ProductId}", input.ProductId);

            var created = await GetByProductIdAsync(input.ProductId);
            return (true, null, created);
        }

        public async Task<(bool IsSuccess, string? Error)> UpdateStockAsync(int productId, UpdateInventoryStockDto input)
        {
            var inventory = await _context.Inventory.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null)
            {
                _logger.LogWarning("Inventory update failed. ProductId not found: {ProductId}", productId);
                return (false, "Inventory record not found.");
            }

            inventory.StockQuantity = input.StockQuantity;
            inventory.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inventory updated. ProductId: {ProductId}, NewStock: {StockQuantity}", productId, input.StockQuantity);
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? Error)> DeleteByProductIdAsync(int productId)
        {
            var inventory = await _context.Inventory.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null)
            {
                _logger.LogWarning("Inventory delete failed. ProductId not found: {ProductId}", productId);
                return (false, "Inventory record not found.");
            }

            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Inventory deleted for ProductId: {ProductId}", productId);

            return (true, null);
        }
    }
}
