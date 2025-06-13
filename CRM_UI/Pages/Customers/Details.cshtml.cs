using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM_UI.Models;
using CRM_UI.Services;

namespace CRM_UI.Pages.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<DetailsModel> _logger;

        public Customer? Customer { get; private set; }

        public DetailsModel(CustomerService customerService, ILogger<DetailsModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Customer = await _customerService.GetCustomerByIdAsync(id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
} 