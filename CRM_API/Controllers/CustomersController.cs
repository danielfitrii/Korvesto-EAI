using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace CRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private static List<Customer> _customers = new()
        {
            new Customer { CustomerId = "C001", Name = "Alice", Email = "alice@email.com", LoyaltyPoints = 100 },
            new Customer { CustomerId = "C002", Name = "Bob", Email = "bob@email.com", LoyaltyPoints = 200 }
        };

        [HttpGet]
        public IActionResult GetAll() => Ok(_customers);

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId == id);
            return customer == null ? NotFound() : Ok(customer);
        }

        [HttpPost("{customerId}/add-points/{points}")]
        public IActionResult AddPoints(string customerId, int points)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null) return NotFound("Customer not found.");

            customer.LoyaltyPoints += points;
            return Ok(customer);
        }
    }
}
