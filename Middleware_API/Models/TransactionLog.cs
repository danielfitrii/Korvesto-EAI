namespace Middleware_API.Models
{
    public class TransactionLog
    {
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string Operation { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 