namespace Middleware_UI.Models
{
    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
} 