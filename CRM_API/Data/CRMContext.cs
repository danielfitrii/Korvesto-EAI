using Microsoft.EntityFrameworkCore;
using Korvesto.Shared.Models;

namespace CRM_API.Data
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for Customer if needed, or other model configurations
            // For Customer, assuming CustomerId is already a primary key
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        }

        public static void SeedData(CRMContext context)
        {
            context.Database.EnsureCreated(); // Ensure the database is created

            if (!context.Customers.Any())
            {
                context.Customers.AddRange(
                    new Customer { CustomerId = "CUST001", Name = "John Doe", Email = "john@example.com", Phone = "111-222-3333", LoyaltyPoints = 100, JoinDate = DateTime.UtcNow.AddMonths(-12) },
                    new Customer { CustomerId = "CUST002", Name = "Jane Smith", Email = "jane@example.com", Phone = "222-333-4444", LoyaltyPoints = 250, JoinDate = DateTime.UtcNow.AddMonths(-10) },
                    new Customer { CustomerId = "CUST003", Name = "Bob Johnson", Email = "bob@example.com", Phone = "333-444-5555", LoyaltyPoints = 50, JoinDate = DateTime.UtcNow.AddMonths(-8) },
                    new Customer { CustomerId = "CUST004", Name = "Alice Brown", Email = "alice@example.com", Phone = "444-555-6666", LoyaltyPoints = 150, JoinDate = DateTime.UtcNow.AddMonths(-7) },
                    new Customer { CustomerId = "CUST005", Name = "Charlie Davis", Email = "charlie@example.com", Phone = "555-666-7777", LoyaltyPoints = 300, JoinDate = DateTime.UtcNow.AddMonths(-5) },
                    new Customer { CustomerId = "CUST006", Name = "Emily White", Email = "emily@example.com", Phone = "666-777-8888", LoyaltyPoints = 80, JoinDate = DateTime.UtcNow.AddMonths(-4) },
                    new Customer { CustomerId = "CUST007", Name = "David Green", Email = "david@example.com", Phone = "777-888-9999", LoyaltyPoints = 120, JoinDate = DateTime.UtcNow.AddMonths(-3) },
                    new Customer { CustomerId = "CUST008", Name = "Sophia Black", Email = "sophia@example.com", Phone = "888-999-0000", LoyaltyPoints = 180, JoinDate = DateTime.UtcNow.AddMonths(-2) },
                    new Customer { CustomerId = "CUST009", Name = "James Blue", Email = "james@example.com", Phone = "999-000-1111", LoyaltyPoints = 70, JoinDate = DateTime.UtcNow.AddMonths(-1) },
                    new Customer { CustomerId = "CUST010", Name = "Olivia Grey", Email = "olivia@example.com", Phone = "000-111-2222", LoyaltyPoints = 200, JoinDate = DateTime.UtcNow }
                );
                context.SaveChanges();
            }
        }
    }
} 