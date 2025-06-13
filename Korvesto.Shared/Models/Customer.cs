namespace Korvesto.Shared.Models
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int LoyaltyPoints { get; set; }
        public DateTime JoinDate { get; set; }
    }

    public class LoyaltyTransaction
    {
        public string CustomerId { get; set; }
        public int Points { get; set; }  // Positive for earning, negative for spending
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
} 