using System.ComponentModel.DataAnnotations;

namespace MVCPractice.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100000")]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; }

        public bool IsAvailable { get; set; }
    }
}
