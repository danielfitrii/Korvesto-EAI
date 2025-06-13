using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Middleware_UI.Services;
using Middleware_UI.Models;

namespace Middleware_UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MiddlewareService _middlewareService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(MiddlewareService middlewareService, ILogger<IndexModel> logger)
        {
            _middlewareService = middlewareService;
            _logger = logger;
        }

        public List<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

        public async Task OnGetAsync()
        {
            try
            {
                ActivityLogs = await _middlewareService.GetActivityLogsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching activity logs");
                ActivityLogs = new List<ActivityLog>();
            }
        }
    }
}
