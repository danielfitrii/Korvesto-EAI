using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using POS_UI.Models;
using POS_UI.Services;

namespace POS_UI.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductService _productService;

        public Product Product { get; set; }

        public DetailsModel(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
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