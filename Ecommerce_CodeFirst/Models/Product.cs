using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this using
namespace Ecommerce_CodeFirst.Models;   

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")] // This tells SQL: 18 digits total, 2 after decimal
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Category { get; set; }
}