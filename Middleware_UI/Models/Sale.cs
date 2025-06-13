namespace Middleware_UI.Models
{
    public class Sale
    {
        public string? Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();
    }

    public class SaleItem
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 