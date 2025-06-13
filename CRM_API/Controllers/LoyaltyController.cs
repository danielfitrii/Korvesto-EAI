using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using static CRM_API.Controllers.CustomersController;

namespace CRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController : ControllerBase
    {
        private static List<LoyaltyTransaction> _transactions = new();

        [HttpPost("{customerId}/points")]
        public IActionResult UpdateLoyaltyPoints(string customerId, [FromBody] LoyaltyTransaction transaction)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
                return NotFound($"Customer {customerId} not found");

            if (transaction == null)
                return BadRequest("Transaction data is required");

            // Update customer's loyalty points
            customer.LoyaltyPoints += transaction.Points;

            // Record the transaction
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