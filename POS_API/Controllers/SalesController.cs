using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace POS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private static List<Sale> _sales = new List<Sale>
        {
            new Sale
            {
                Id = 1,
                Date = DateTime.Now,
                CustomerName = "Alice",
                TotalAmount = 150.00m,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductCode = "P001", ProductName = "Mouse", Quantity = 2, Price = 25.00m },
                    new SaleItem { ProductCode = "P002", ProductName = "Keyboard", Quantity = 1, Price = 100.00m }
                }
            }
        };

        // GET: api/sales
        [HttpGet]
        public IActionResult GetSales()
        {
            return Ok(_sales);
        }

        // POST: api/sales
        [HttpPost]
        public IActionResult CreateSale([FromBody] Sale newSale)
        {
            // Generate a simple ID for in-memory storage
            newSale.Id = _sales.Any() ? _sales.Max(s => s.Id) + 1 : 1;
            _sales.Add(newSale);

            return CreatedAtAction(nameof(GetSales), new { id = newSale.Id }, newSale);
        }
    }
}
