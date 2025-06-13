using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Middleware_UI.Pages.Sales
{
    public class SaleDetailsModel : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public SaleDetailsResponse SaleDetails { get; private set; }

        public SaleDetailsModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SaleDetailsModel> logger)
            : base(httpClientFactory, configuration, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                SetErrorMessage("Sale ID is required");
                return RedirectToPage("/Index");
            }

            try
            {
                using var client = CreateAuthenticatedClient();
                var response = await client.GetAsync($"api/sale/{Id}/details");

                if (response.IsSuccessStatusCode)
                {
                    SaleDetails = await response.Content.ReadFromJsonAsync<SaleDetailsResponse>();
                    return Page();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    SetErrorMessage("Sale not found");
                    return Page();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    SetErrorMessage($"Failed to retrieve sale details: {error}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return HandleApiError(ex);
            }
        }
    }

    public class SaleDetailsResponse
    {
        public string SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDetails> Items { get; set; }
        public CustomerDetails Customer { get; set; }
        public LoyaltyDetails Loyalty { get; set; }
    }

    public class SaleItemDetails
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CustomerDetails
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class LoyaltyDetails
    {
        public int CurrentBalance { get; set; }
        public List<LoyaltyTransaction> RecentTransactions { get; set; }
    }

    public class LoyaltyTransaction
    {
        public int Points { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 