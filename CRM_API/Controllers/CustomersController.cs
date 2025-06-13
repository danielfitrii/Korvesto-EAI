using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace CRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        // Made public static to allow access from LoyaltyController for simplicity in this in-memory setup.
        public static List<Customer> _customers = new()
        {
            new Customer 
            { 
                CustomerId = "CUST001", 
                Name = "John Doe", 
                Email = "john@example.com",
                Phone = "123-456-7890",
                LoyaltyPoints = 100,
                JoinDate = DateTime.UtcNow.AddMonths(-1)
            },
            new Customer 
            { 
                CustomerId = "CUST002", 
                Name = "Jane Smith", 
                Email = "jane@example.com",
                Phone = "098-765-4321",
                LoyaltyPoints = 250,
                JoinDate = DateTime.UtcNow.AddMonths(-2)
            },
            new Customer 
            { 
                CustomerId = "CUST007", 
                Name = "Mystery Customer", 
                Email = "cust007@example.com",
                Phone = "777-777-7777",
                LoyaltyPoints = 0,
                JoinDate = DateTime.UtcNow
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_customers);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return NotFound($"Customer {id} not found");

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required");

            if (string.IsNullOrEmpty(customer.CustomerId))
                return BadRequest("CustomerId is required");

            if (_customers.Any(c => c.CustomerId == customer.CustomerId))
                return BadRequest($"Customer with ID {customer.CustomerId} already exists");

            customer.JoinDate = DateTime.UtcNow;
            customer.LoyaltyPoints = 0;
            _customers.Add(customer);

            return CreatedAtAction(nameof(Get), new { id = customer.CustomerId }, customer);
        }
    }
} 