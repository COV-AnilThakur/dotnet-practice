using System.ComponentModel.DataAnnotations;

namespace Ecommerce_DBFirst.ViewModels
{
    public class InventoryUpdateVM
    {
        [Required]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}
