using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;

namespace POS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new()
        {
            new Product 
            { 
                Id = "P001", 
                Name = "Laptop", 
                Price = 49.99m, 
                Description = "High-performance laptop",
                StockQuantity = 10
            },
            new Product 
            { 
                Id = "P002", 
                Name = "Smartphone", 
                Price = 49.99m, 
                Description = "Latest model smartphone",
                StockQuantity = 15
            },
            new Product 
            { 
                Id = "P003", 
                Name = "Headphones", 
                Price = 29.99m, 
                Description = "Wireless noise-cancelling headphones",
                StockQuantity = 20
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
} 