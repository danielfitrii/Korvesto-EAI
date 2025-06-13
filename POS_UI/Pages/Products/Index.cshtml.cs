using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using POS_UI.Models;
using POS_UI.Services;

namespace POS_UI.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductService _productService;
        public List<Product> Products { get; set; } = new List<Product>();

        public IndexModel(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Products = await _productService.GetAllProductsAsync();
            return Page();
        }
    }
} 