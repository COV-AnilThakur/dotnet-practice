using Ecommerce_CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Ecommerce_CodeFirst.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}