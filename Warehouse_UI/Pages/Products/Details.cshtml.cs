using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Warehouse_UI.Services;
using Warehouse_UI.Models;

namespace Warehouse_UI.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly ILogger<DetailsModel> _logger;

        public Product? Product { get; private set; }

        public DetailsModel(ProductService productService, ILogger<DetailsModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Product = await _productService.GetProductByIdAsync(id);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
} 