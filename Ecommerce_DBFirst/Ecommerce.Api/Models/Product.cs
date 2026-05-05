namespace Ecommerce.Api.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
