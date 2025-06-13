using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using POS_API.Data;
using Microsoft.EntityFrameworkCore;

namespace POS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly POSContext _context;

        public SalesController(POSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _context.Sales.Include(s => s.Items).ToListAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sale sale)
        {
            Console.WriteLine($"API Received Sale: {(sale != null ? "Not Null" : "Null")}");
            if (sale != null)
            {
                Console.WriteLine($"API Received CustomerId: {sale.CustomerId ?? "Null"}");
                Console.WriteLine($"API Received Items Count: {sale.Items?.Count ?? 0}");
            }

            // Check model state for any binding or validation errors
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (sale == null)
                return BadRequest("Sale data is required");

            if (string.IsNullOrEmpty(sale.CustomerId))
                return BadRequest("CustomerId is required");

            if (sale.Items == null || !sale.Items.Any())
                return BadRequest("Sale must contain at least one item");

            // Check if ID is provided, if not generate one
            if (string.IsNullOrEmpty(sale.Id))
            {
                sale.Id = Guid.NewGuid().ToString();
            }
            else
            {
                // Check if ID already exists
                if (await _context.Sales.AnyAsync(s => s.Id == sale.Id))
                {
                    return BadRequest($"Sale with ID {sale.Id} already exists");
                }
            }

            sale.SaleDate = DateTime.UtcNow;

            // Calculate total amount
            sale.TotalAmount = sale.Items.Sum(item => item.TotalPrice);

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = sale.Id }, sale);
        }
    }
}
