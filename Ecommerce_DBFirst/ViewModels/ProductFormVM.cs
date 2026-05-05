using System.ComponentModel.DataAnnotations;

namespace Ecommerce_DBFirst.ViewModels
{
    public class ProductFormVM
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public int? CategoryId { get; set; }
    }
}
