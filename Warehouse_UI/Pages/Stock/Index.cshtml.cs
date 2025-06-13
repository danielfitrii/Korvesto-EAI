using Microsoft.AspNetCore.Mvc.RazorPages;
using Warehouse_UI.Models;
using Warehouse_UI.Services;

namespace Warehouse_UI.Pages.Stock
{
    public class IndexModel : PageModel
    {
        private readonly StockService _stockService;
        public List<StockItem> StockList { get; set; } = new List<StockItem>();

        public IndexModel(StockService stockService)
        {
            _stockService = stockService;
        }

        public async Task OnGetAsync()
        {
            StockList = await _stockService.GetAllStockAsync();
        }
    }
} 