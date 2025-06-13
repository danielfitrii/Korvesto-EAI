using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using POS_UI.Models;
using POS_UI.Services;
using Microsoft.Extensions.Logging;

namespace POS_UI.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly SaleService _saleService;
        private readonly ProductService _productService;
        private readonly ILogger<CreateModel> _logger;

        public List<Product> Products { get; set; } = new List<Product>();

        [BindProperty]
        public Sale Sale { get; set; }

        public CreateModel(SaleService saleService, ProductService productService, ILogger<CreateModel> logger)
        {
            _saleService = saleService;
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Products = await _productService.GetAllProductsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync started.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: {ModelStateErrors}",
                    string.Join("; ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)));
                Products = await _productService.GetAllProductsAsync();
                return Page();
            }
            _logger.LogInformation("ModelState is valid.");

            if (Sale.Items == null || !Sale.Items.Any())
            {
                _logger.LogWarning("Sale.Items is null or empty.");
                ModelState.AddModelError("", "At least one item is required");
                Products = await _productService.GetAllProductsAsync();
                return Page();
            }
            _logger.LogInformation($"Sale contains {Sale.Items.Count} items.");

            // Calculate TotalAmount and TotalPrice for each SaleItem on the server side
            // This provides an extra layer of security and ensures consistency with API expectations
            decimal calculatedTotalAmount = 0;
            foreach (var item in Sale.Items)
            {
                // Fetch the product to get the actual unit price
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                    Products = await _productService.GetAllProductsAsync();
                    return Page();
                }
                item.UnitPrice = product.Price;
                item.TotalPrice = item.Quantity * item.UnitPrice;
                calculatedTotalAmount += item.TotalPrice;
            }
            Sale.TotalAmount = calculatedTotalAmount;

            // Explicitly set SaleDate before sending to API to avoid potential model binding issues
            Sale.SaleDate = DateTime.UtcNow;

            _logger.LogInformation($"Calculated TotalAmount: {Sale.TotalAmount}, SaleDate: {Sale.SaleDate}");

            var createdSale = await _saleService.CreateSaleAsync(Sale);
            if (createdSale == null)
            {
                ModelState.AddModelError("", "Error creating sale. Please check the logs for more details.");
                Products = await _productService.GetAllProductsAsync();
                return Page();
            }

            TempData["SuccessMessage"] = $"Sale with ID {createdSale.Id} created successfully!";
            _logger.LogInformation($"Sale {createdSale.Id} created successfully. Redirecting to Index.");
            return RedirectToPage("./Index");
        }
    }
} 