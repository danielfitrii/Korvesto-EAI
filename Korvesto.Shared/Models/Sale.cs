namespace Korvesto.Shared.Models
{
    public class Sale
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();
    }

    public class SaleItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
