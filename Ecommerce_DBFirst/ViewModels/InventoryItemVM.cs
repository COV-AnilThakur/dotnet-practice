namespace Ecommerce_DBFirst.ViewModels
{
    public class InventoryItemVM
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
