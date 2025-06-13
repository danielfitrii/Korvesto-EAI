using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Warehouse_UI.Models;
using Warehouse_UI.Services;

namespace Warehouse_UI.Pages.Stock
{
    public class AdjustModel : PageModel
    {
        private readonly StockService _stockService;
        private readonly ILogger<AdjustModel> _logger;

        [BindProperty]
        public StockAdjustment Adjustment { get; set; } = new();

        public StockItem? CurrentStock { get; private set; }

        public AdjustModel(StockService stockService, ILogger<AdjustModel> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string? productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return RedirectToPage("./Index");
            }

            Adjustment.ProductId = productId;
            Adjustment.Location = "Main Warehouse"; // Default location

            // Get current stock level
            var stockList = await _stockService.GetAllStockAsync();
            CurrentStock = stockList.FirstOrDefault(s => s.ProductId == productId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _stockService.AdjustStockAsync(Adjustment);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                ModelState.AddModelError("", "Failed to adjust stock. Please try again.");
                return Page();
            }
        }
    }
} 