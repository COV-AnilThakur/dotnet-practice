using Microsoft.AspNetCore.Mvc;
using Ecommerce_CodeFirst.Data;
using Ecommerce_CodeFirst.Models;
using System.Linq;

namespace Ecommerce_CodeFirst.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Retrieval: List all products
        public IActionResult Index()
        {
            var productList = _context.Products.ToList();
            return View(productList);
        }

        // 2. Insertion: GET Form
        public IActionResult Create()
        {
            return View();
        }

        // 3. Insertion: POST Form
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product); // Adds to tracking
                _context.SaveChanges();         // Saves to SQL Server
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}