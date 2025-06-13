using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using CRM_API.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController : ControllerBase
    {
        private readonly CRMContext _context;
        private static List<LoyaltyTransaction> _transactions = new();

        public LoyaltyController(CRMContext context)
        {
            _context = context;
        }

        [HttpPost("{customerId}/points")]
        public async Task<IActionResult> UpdateLoyaltyPoints(string customerId, [FromBody] LoyaltyTransaction transaction)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
                return NotFound($"Customer {customerId} not found");

            if (transaction == null)
                return BadRequest("Transaction data is required");

            // Update customer's loyalty points
            customer.LoyaltyPoints += transaction.Points;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Record the transaction (currently still in-memory)
            transaction.CustomerId = customerId;
            transaction.TransactionDate = DateTime.UtcNow;
            _transactions.Add(transaction);

            return Ok(new { 
                CustomerId = customer.CustomerId, 
                NewPoints = customer.LoyaltyPoints,
                Transaction = transaction 
            });
        }
    }
} 