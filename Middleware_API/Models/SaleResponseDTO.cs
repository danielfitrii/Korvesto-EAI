namespace Middleware_API.Models
{
    public class SaleResponseDTO
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public SaleDetailsDTO SaleDetails { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SaleDetailsDTO
    {
        public string SaleId { get; set; }
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int LoyaltyPointsEarned { get; set; }
        public DateTime TransactionDate { get; set; }
    }
} 