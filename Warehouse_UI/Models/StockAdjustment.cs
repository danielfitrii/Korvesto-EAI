namespace Warehouse_UI.Models
{
    public class StockAdjustment
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }  // Positive for addition, negative for deduction
        public string Location { get; set; }
        public string Reason { get; set; }
    }
} 