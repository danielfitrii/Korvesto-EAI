using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM_UI.Models;
using CRM_UI.Services;

namespace CRM_UI.Pages.Customers
{
    public class AddLoyaltyPointsModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<AddLoyaltyPointsModel> _logger;

        public AddLoyaltyPointsModel(CustomerService customerService, ILogger<AddLoyaltyPointsModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        [BindProperty]
        public int Points { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Points <= 0)
            {
                ModelState.AddModelError("Points", "Points must be greater than 0");
                return Page();
            }

            var updatedCustomer = await _customerService.AddLoyaltyPointsAsync(Customer.CustomerId, Points);
            if (updatedCustomer == null)
            {
                ModelState.AddModelError("", "Failed to add loyalty points. Please try again.");
                return Page();
            }

            return RedirectToPage("./Details", new { id = Customer.CustomerId });
        }
    }
} 