using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Middleware_UI.Pages.Sales
{
    public class SaleFormModel : BasePageModel
    {
        [BindProperty]
        public SaleRequest SaleRequest { get; set; } = new();

        public SaleFormModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SaleFormModel> logger)
            : base(httpClientFactory, configuration, logger)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using var client = CreateAuthenticatedClient();
                var response = await client.PostAsJsonAsync("api/sale/process-sale", SaleRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SaleResponse>();
                    SetSuccessMessage($"Sale processed successfully. Transaction ID: {result.TransactionId}");
                    return RedirectToPage("/Sales/SaleDetails", new { id = result.TransactionId });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    SetErrorMessage($"Failed to process sale: {error}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return HandleApiError(ex);
            }
        }
    }

    public class SaleRequest
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class SaleResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public SaleDetails SaleDetails { get; set; }
        public List<string> Errors { get; set; }
    }

    public class SaleDetails
    {
        public string SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
    }
} 