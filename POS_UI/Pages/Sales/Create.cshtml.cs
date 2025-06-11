using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Korvesto.Shared.Models;
using System.Text.Json;

namespace POS_UI.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public Sale NewSale { get; set; } = new Sale { Items = new List<SaleItem>() };

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("POSApi");
        }

        public IActionResult OnGet()
        {
            NewSale.Date = DateTime.Now; // Pre-fill with current date
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Calculate TotalAmount before sending (if not calculated in API)
            NewSale.TotalAmount = NewSale.Items.Sum(item => item.Quantity * item.Price);
            
            var jsonContent = JsonSerializer.Serialize(NewSale);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/sales", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error creating sale: {response.StatusCode} - {errorContent}");
                return Page();
            }
        }
    }
} 