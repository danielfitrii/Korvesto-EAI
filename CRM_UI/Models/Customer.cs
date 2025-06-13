namespace CRM_UI.Models
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
} 