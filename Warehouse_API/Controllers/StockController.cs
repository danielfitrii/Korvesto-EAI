using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace Warehouse_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private static List<Stock> _stock = new()
        {
            new Stock 
            { 
                ProductId = "P001", 
                Quantity = 10, 
                Location = "Main Warehouse",
                LastUpdated = DateTime.UtcNow
            },
            new Stock 
            { 
                ProductId = "P002", 
                Quantity = 15, 
                Location = "Main Warehouse",
                LastUpdated = DateTime.UtcNow
            },
            new Stock 
            { 
                ProductId = "P003", 
                Quantity = 20, 
                Location = "Main Warehouse",
                LastUpdated = DateTime.UtcNow
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_stock);
        }

        [HttpGet("{productId}")]
        public IActionResult Get(string productId)
        {
            var stock = _stock.FirstOrDefault(s => s.ProductId == productId);
            if (stock == null)
                return NotFound($"No stock found for product {productId}");

            return Ok(stock);
        }

        [HttpPost("adjust")]
        public IActionResult AdjustStock([FromBody] StockAdjustment adjustment)
        {
            if (adjustment == null)
                return BadRequest("Adjustment data is required");

            var stock = _stock.FirstOrDefault(s => s.ProductId == adjustment.ProductId);
            
            if (stock == null)
            {
                // Create new stock entry if it doesn't exist
                stock = new Stock
                {
                    ProductId = adjustment.ProductId,
                    Quantity = 0,
                    Location = adjustment.Location,
                    LastUpdated = DateTime.UtcNow
                };
                _stock.Add(stock);
            }

            // Update stock
            stock.Quantity += adjustment.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            stock.Location = adjustment.Location;

            return Ok(stock);
        }
    }
}
