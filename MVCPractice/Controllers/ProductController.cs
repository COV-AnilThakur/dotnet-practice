using Microsoft.AspNetCore.Mvc;
using MVCPractice.Models;

namespace MVCPractice.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {
        private static List<Product> products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 75000, Category = "Electronics", IsAvailable = true },
            new Product { Id = 2, Name = "Mobile", Price = 30000, Category = "Electronics", IsAvailable = true }

        };

        // GET: /Product
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.Products = products;
            return View();
        }

        // GET: /Product/1
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.Product = products.FirstOrDefault(p => p.Id == id);
            return View();
        }

        //GET Create Form
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        //POST Create Form
        [HttpPost("create")]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            product.Id = products.Count + 1;
            products.Add(product);

            return RedirectToAction("Index");
        }
    }
}
