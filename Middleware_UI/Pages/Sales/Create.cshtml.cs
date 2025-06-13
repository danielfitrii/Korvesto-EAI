using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Middleware_UI.Services;
using Middleware_UI.Models;

namespace Middleware_UI.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly MiddlewareService _middlewareService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(MiddlewareService middlewareService, ILogger<CreateModel> logger)
        {
            _middlewareService = middlewareService;
            _logger = logger;
        }

        [BindProperty]
        public Sale Sale { get; set; } = new Sale
        {
            SaleDate = DateTime.UtcNow
        };

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Customer> Customers { get; set; } = new List<Customer>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Products = await _middlewareService.GetProductsAsync();
                Customers = await _middlewareService.GetCustomersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products or customers");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Products = await _middlewareService.GetProductsAsync();
                Customers = await _middlewareService.GetCustomersAsync();
                return Page();
            }

            try
            {
                var success = await _middlewareService.ProcessSaleAsync(Sale);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to process the sale.");
                    Products = await _middlewareService.GetProductsAsync();
                    return Page();
                }

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sale");
                ModelState.AddModelError("", "An error occurred while processing the sale.");
                Products = await _middlewareService.GetProductsAsync();
                return Page();
            }
        }
    }
} 