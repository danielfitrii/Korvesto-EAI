using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using POS_UI.Models;
using POS_UI.Services;

namespace POS_UI.Pages.Sales
{
    public class DetailsModel : PageModel
    {
        private readonly SaleService _saleService;

        public Sale Sale { get; set; }

        public DetailsModel(SaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale = await _saleService.GetSaleByIdAsync(id);

            if (Sale == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
} 