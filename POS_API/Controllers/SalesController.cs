using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace POS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private static List<Sale> _sales = new();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_sales);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Sale sale)
        {
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
                if (_sales.Any(s => s.Id == sale.Id))
                {
                    return BadRequest($"Sale with ID {sale.Id} already exists");
                }
            }

            sale.SaleDate = DateTime.UtcNow;

            // Calculate total amount
            sale.TotalAmount = sale.Items.Sum(item => item.TotalPrice);

            _sales.Add(sale);
            return CreatedAtAction(nameof(Get), new { id = sale.Id }, sale);
        }
    }
}
