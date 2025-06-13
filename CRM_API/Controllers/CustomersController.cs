using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using CRM_API.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CRMContext _context;

        public CustomersController(CRMContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Customers.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return NotFound($"Customer {id} not found");

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required");

            if (string.IsNullOrEmpty(customer.CustomerId))
                return BadRequest("CustomerId is required");

            if (await _context.Customers.AnyAsync(c => c.CustomerId == customer.CustomerId))
                return BadRequest($"Customer with ID {customer.CustomerId} already exists");

            customer.JoinDate = DateTime.UtcNow;
            customer.LoyaltyPoints = 0;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = customer.CustomerId }, customer);
        }
    }
} 