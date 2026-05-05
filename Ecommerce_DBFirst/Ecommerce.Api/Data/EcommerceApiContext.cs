using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data
{
    public class EcommerceApiContext : DbContext
    {
        public EcommerceApiContext(DbContextOptions<EcommerceApiContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Inventory> Inventory => Set<Inventory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.ToTable("Products");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InventoryId);
                entity.Property(e => e.StockQuantity).IsRequired();
                entity.Property(e => e.LastUpdated).HasColumnType("datetime");
                entity.ToTable("Inventory");

                entity.HasIndex(e => e.ProductId).IsUnique();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
