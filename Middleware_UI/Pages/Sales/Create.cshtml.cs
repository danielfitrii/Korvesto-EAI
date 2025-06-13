using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Middleware_UI.Models;
using Middleware_UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Middleware_UI.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly MiddlewareService _middlewareService;
        private readonly ILogger<CreateModel> _logger;
        private readonly CustomerLookupService _customerLookupService;
        private readonly ProductLookupService _productLookupService;

        [BindProperty]
        public Sale Sale { get; set; }

        [BindProperty]
        public SaleItem NewItem { get; set; } = new SaleItem();

        public string? ResultMessage { get; set; }

        public List<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();

        public string ErrorMessage { get; set; }

        public CreateModel(MiddlewareService middlewareService, ILogger<CreateModel> logger, CustomerLookupService customerLookupService, ProductLookupService productLookupService)
        {
            _middlewareService = middlewareService;
            _logger = logger;
            _customerLookupService = customerLookupService;
            _productLookupService = productLookupService;
            Sale = new Sale(); // Initialize Sale here
        }

        public async Task OnGetAsync()
        {
            await LoadAvailableProducts();
            // Initialize with empty sale
        }

        private async Task LoadAvailableProducts()
        {
            var products = await _productLookupService.GetAllProductsAsync();
            AvailableProducts = products.Select(p => new SelectListItem { Value = p.Id, Text = p.Name + " (" + p.Price.ToString("C") + ")" }).ToList();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnGetGetCustomerInfo(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return new JsonResult(new { });
            var customer = await _customerLookupService.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return new JsonResult(new { });
            return new JsonResult(customer);
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnGetGetProductInfo(string? productId = null)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                var allProducts = await _productLookupService.GetAllProductsAsync();
                return new JsonResult(allProducts);
            }
            else
            {
                var product = await _productLookupService.GetProductByIdAsync(productId);
                if (product == null)
                    return new JsonResult(new { });
                return new JsonResult(product);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableProducts();
                // Manually re-serialize Sale.Items to retain state on validation error
                if (Sale.Items != null && Sale.Items.Any())
                {
                    ViewData["SaleItemsJson"] = JsonConvert.SerializeObject(Sale.Items);
                }
                return Page();
            }

            if (Sale.Items == null || !Sale.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Please add at least one item to the sale.");
                await LoadAvailableProducts();
                // Manually re-serialize Sale.Items to retain state on validation error
                if (Sale.Items != null && Sale.Items.Any())
                {
                    ViewData["SaleItemsJson"] = JsonConvert.SerializeObject(Sale.Items);
                }
                return Page();
            }

            try
            {
                // The Sale.Items are now automatically bound from the hidden JSON input
                // when the form is submitted, because Sale is marked with [BindProperty].
                // No explicit deserialization is needed here if the Sale.Items property
                // in the Sale model is of type List<SaleItem>.

                Sale.Id = Guid.NewGuid().ToString(); // Generate a new ID for the sale
                Sale.SaleDate = DateTime.UtcNow;
                Sale.TotalAmount = Sale.Items.Sum(item => item.TotalPrice); // Recalculate total for safety

                var success = await _middlewareService.ProcessSaleAsync(Sale);

                if (success)
                {
                    TempData["SuccessMessage"] = "Sale processed successfully!";
                    return RedirectToPage("/Sales/Create");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error processing sale.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sale.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
            }

            await LoadAvailableProducts();
            // Manually re-serialize Sale.Items to retain state on validation error
            if (Sale.Items != null && Sale.Items.Any())
            {
                ViewData["SaleItemsJson"] = JsonConvert.SerializeObject(Sale.Items);
            }
            return Page();
        }
    }
} 