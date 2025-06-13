namespace Middleware_API.Models
{
    public class SaleDetailsResponseDTO
    {
        public string SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDetailsDTO> Items { get; set; }
        public CustomerDetailsDTO Customer { get; set; }
        public LoyaltyDetailsDTO Loyalty { get; set; }
    }

    public class SaleItemDetailsDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CustomerDetailsDTO
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class LoyaltyDetailsDTO
    {
        public int PointsEarned { get; set; }
        public int CurrentBalance { get; set; }
        public string TransactionDescription { get; set; }
    }
} 