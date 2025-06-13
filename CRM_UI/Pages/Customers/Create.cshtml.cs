using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM_UI.Models;
using CRM_UI.Services;

namespace CRM_UI.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public CreateModel(CustomerService customerService, ILogger<CreateModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set default values
            Customer.JoinDate = DateTime.UtcNow;
            Customer.LoyaltyPoints = 0;

            var createdCustomer = await _customerService.CreateCustomerAsync(Customer);

            if (createdCustomer == null)
            {
                ModelState.AddModelError("", "Failed to create customer. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
} 