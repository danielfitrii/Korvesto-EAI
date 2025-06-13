using Microsoft.EntityFrameworkCore;
using Korvesto.Shared.Models;

namespace POS_API.Data
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options) : base(options)
        {
        }

        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>().HasKey(s => s.Id);
            modelBuilder.Entity<Sale>().Property(s => s.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<SaleItem>().HasKey(si => new { si.SaleId, si.ProductId });
        }
    }
} 