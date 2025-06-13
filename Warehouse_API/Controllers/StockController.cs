using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Warehouse_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private static Dictionary<string, int> _stockLevels = new();

        [HttpGet]
        public IActionResult GetAllStock()
        {
            return Ok(_stockLevels);
        }

        [HttpPost("{productCode}/deduct/{quantity}")]
        public IActionResult DeductStock(string productCode, int quantity)
        {
            if (!_stockLevels.ContainsKey(productCode))
                return NotFound("Product not found.");

            _stockLevels[productCode] -= quantity;
            return Ok(new { ProductCode = productCode, RemainingStock = _stockLevels[productCode] });
        }

        [HttpPost("{productCode}/add/{quantity}")]
        public IActionResult AddStock(string productCode, int quantity)
        {
            if (!_stockLevels.ContainsKey(productCode))
            {
                _stockLevels[productCode] = quantity;
            }
            else
            {
                _stockLevels[productCode] += quantity;
            }
            return Ok(new { ProductCode = productCode, RemainingStock = _stockLevels[productCode] });
        }
    }
}
