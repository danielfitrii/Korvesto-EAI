namespace Korvesto.Shared.Models
{
    public class Stock
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class StockAdjustment
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }  // Positive for addition, negative for deduction
        public string Location { get; set; }
        public string Reason { get; set; }
    }
} 