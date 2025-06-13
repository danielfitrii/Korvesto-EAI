using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using Warehouse_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly WarehouseContext _context;

        public StockController(WarehouseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Stock.ToListAsync());
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(string productId)
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(s => s.ProductId == productId);
            if (stock == null)
                return NotFound($"No stock found for product {productId}");

            return Ok(stock);
        }

        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock([FromBody] StockAdjustment adjustment)
        {
            if (adjustment == null)
                return BadRequest("Adjustment data is required");

            var stock = await _context.Stock.FirstOrDefaultAsync(s => s.ProductId == adjustment.ProductId);
            
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
                _context.Stock.Add(stock);
            }

            // Update stock
            stock.Quantity += adjustment.Quantity;
            stock.LastUpdated = DateTime.UtcNow;
            stock.Location = adjustment.Location;

            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        // Optional: Endpoint to initialize data for testing
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeData()
        {
            if (await _context.Stock.AnyAsync())
            {
                return BadRequest("Stock data already exists. Use PUT /api/Stock/reset to clear and re-initialize.");
            }

            var initialStock = new List<Stock>
            {
                new Stock { ProductId = "P001", Quantity = 50, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P002", Quantity = 75, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P003", Quantity = 120, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P004", Quantity = 40, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P005", Quantity = 60, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P006", Quantity = 30, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P007", Quantity = 150, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P008", Quantity = 80, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P009", Quantity = 70, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow },
                new Stock { ProductId = "P010", Quantity = 200, Location = "Main Warehouse", LastUpdated = DateTime.UtcNow }
            };

            _context.Stock.AddRange(initialStock);
            await _context.SaveChangesAsync();

            return Ok("Stock data initialized successfully.");
        }
    }
}
