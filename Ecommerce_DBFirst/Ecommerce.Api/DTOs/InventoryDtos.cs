using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.DTOs
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class CreateInventoryDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }

    public class UpdateInventoryStockDto
    {
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}
