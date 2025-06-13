using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM_UI.Models;
using CRM_UI.Services;

namespace CRM_UI.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<IndexModel> _logger;

        public List<Customer> Customers { get; set; } = new List<Customer>();

        public IndexModel(CustomerService customerService, ILogger<IndexModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Customers = await _customerService.GetAllCustomersAsync();
            return Page();
        }
    }
} 