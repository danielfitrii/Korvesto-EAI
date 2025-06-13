namespace Warehouse_UI.Models
{
    public class StockItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
    }
} 