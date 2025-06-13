using Microsoft.EntityFrameworkCore;
using Korvesto.Shared.Models;

namespace Warehouse_API.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasKey(s => s.ProductId);
        }
    }
} 