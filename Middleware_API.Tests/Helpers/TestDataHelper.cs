using Middleware_API.Models;

namespace Middleware_API.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static Customer CreateTestCustomer(string customerId = "C001")
        {
            return new Customer
            {
                CustomerId = customerId,
                Name = "Test Customer",
                Email = "test@example.com",
                Phone = "1234567890",
                LoyaltyPoints = 100
            };
        }

        public static Product CreateTestProduct(string productId = "P001")
        {
            return new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.00m,
                Category = "Test Category"
            };
        }

        public static Stock CreateTestStock(string productId = "P001", int quantity = 10)
        {
            return new Stock
            {
                ProductId = productId,
                Quantity = quantity,
                LastUpdated = DateTime.UtcNow
            };
        }

        public static Sale CreateTestSale(string saleId = "S001", string customerId = "C001", string productId = "P001")
        {
            return new Sale
            {
                Id = saleId,
                CustomerId = customerId,
                SaleDate = DateTime.UtcNow,
                Items = new List<SaleItem>
                {
                    new SaleItem
                    {
                        ProductId = productId,
                        Quantity = 2,
                        UnitPrice = 10.00m,
                        TotalPrice = 20.00m
                    }
                },
                TotalAmount = 20.00m
            };
        }

        public static SaleRequestDTO CreateTestSaleRequest(string customerId = "C001", string productId = "P001", int quantity = 2)
        {
            return new SaleRequestDTO
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity
            };
        }

        public static LoyaltyTransaction CreateTestLoyaltyTransaction(string customerId = "C001", int points = 20)
        {
            return new LoyaltyTransaction
            {
                CustomerId = customerId,
                Points = points,
                TransactionType = "Earn",
                Description = "Test transaction",
                Timestamp = DateTime.UtcNow
            };
        }
    }
} 