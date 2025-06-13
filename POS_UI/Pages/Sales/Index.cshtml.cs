using Microsoft.AspNetCore.Mvc.RazorPages;
using POS_UI.Models;
using POS_UI.Services;

namespace POS_UI.Pages.Sales
{
    public class IndexModel : PageModel
    {
        private readonly SaleService _saleService;
        public List<Sale> Sales { get; set; } = new List<Sale>();

        public IndexModel(SaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task OnGetAsync()
        {
            Sales = await _saleService.GetAllSalesAsync();
        }
    }
} 