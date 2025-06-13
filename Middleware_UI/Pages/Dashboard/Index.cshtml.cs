using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Middleware_UI.Pages.Dashboard
{
    public class IndexModel : BasePageModel
    {
        public SalesSummary SalesSummary { get; private set; }
        public StockSummary StockSummary { get; private set; }
        public LoyaltySummary LoyaltySummary { get; private set; }
        public List<RecentSale> RecentSales { get; private set; }

        public IndexModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<IndexModel> logger)
            : base(httpClientFactory, configuration, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using var client = CreateAuthenticatedClient();
                
                // Fetch dashboard data
                var response = await client.GetAsync("api/dashboard/summary");
                if (response.IsSuccessStatusCode)
                {
                    var dashboardData = await response.Content.ReadFromJsonAsync<DashboardResponse>();
                    SalesSummary = dashboardData.SalesSummary;
                    StockSummary = dashboardData.StockSummary;
                    LoyaltySummary = dashboardData.LoyaltySummary;
                    RecentSales = dashboardData.RecentSales;
                    return Page();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    SetErrorMessage($"Failed to load dashboard data: {error}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return HandleApiError(ex);
            }
        }
    }

    public class DashboardResponse
    {
        public SalesSummary SalesSummary { get; set; }
        public StockSummary StockSummary { get; set; }
        public LoyaltySummary LoyaltySummary { get; set; }
        public List<RecentSale> RecentSales { get; set; }
    }

    public class SalesSummary
    {
        public decimal TotalSales { get; set; }
        public int SalesCount { get; set; }
        public decimal DailyTarget { get; set; }
        public int DailyTargetPercentage { get; set; }
    }

    public class StockSummary
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public class LoyaltySummary
    {
        public int TotalPoints { get; set; }
        public int ActiveMembers { get; set; }
        public int PointsIssuedToday { get; set; }
    }

    public class RecentSale
    {
        public string SaleId { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
} 