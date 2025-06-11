namespace Korvesto.Shared.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public List<SaleItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SaleItem
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Customer
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}
