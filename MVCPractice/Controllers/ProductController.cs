using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVCPractice.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
        private static List<dynamic> products = new()
    {
        new { Id = 1, Name = "Laptop", Price = 75000 },
        new { Id = 2, Name = "Mobile Phone", Price = 30000 },
        new { Id = 3, Name = "Headphones", Price = 5000 }
    };

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Products = products;
            return View();
        }

        //[HttpGet("Details/{id}")]
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.Product = products.FirstOrDefault(p => p.Id == id);
            return View();
        }
    }
}
