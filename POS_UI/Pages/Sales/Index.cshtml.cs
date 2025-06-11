using Microsoft.AspNetCore.Mvc.RazorPages;
using Korvesto.Shared.Models;
using System.Text.Json;

namespace POS_UI.Pages.Sales
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Sale> Sales { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("POSApi");
        }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/sales");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Sales = JsonSerializer.Deserialize<List<Sale>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
} 