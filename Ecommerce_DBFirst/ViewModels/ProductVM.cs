namespace Ecommerce_DBFirst.ViewModels
{
    
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty; // For displaying the joined category name
        public int? StockQuantity { get; set; }
        public bool IsOutOfStock => StockQuantity.HasValue && StockQuantity.Value == 0;
    }
}
