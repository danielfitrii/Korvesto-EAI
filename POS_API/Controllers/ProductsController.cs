using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace POS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ProductsController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        private static List<Product> _products = new()
        {
            new Product 
            { 
                Id = "P001", 
                Name = "Laptop", 
                Price = 999.99m, 
                Description = "High-performance laptop",
                StockQuantity = 50
            },
            new Product 
            { 
                Id = "P002", 
                Name = "Smartphone", 
                Price = 499.99m, 
                Description = "Latest model smartphone",
                StockQuantity = 75
            },
            new Product 
            { 
                Id = "P003", 
                Name = "Wireless Headphones", 
                Price = 149.99m, 
                Description = "Premium wireless noise-cancelling headphones",
                StockQuantity = 120
            },
            new Product 
            { 
                Id = "P004", 
                Name = "Tablet", 
                Price = 299.99m, 
                Description = "Compact and powerful tablet",
                StockQuantity = 40
            },
            new Product 
            { 
                Id = "P005", 
                Name = "Smartwatch", 
                Price = 199.99m, 
                Description = "Feature-rich smartwatch with health tracking",
                StockQuantity = 60
            },
            new Product 
            { 
                Id = "P006", 
                Name = "Gaming Console", 
                Price = 399.99m, 
                Description = "Next-gen gaming console",
                StockQuantity = 30
            },
            new Product 
            { 
                Id = "P007", 
                Name = "Portable Speaker", 
                Price = 79.99m, 
                Description = "Compact and powerful portable speaker",
                StockQuantity = 150
            },
            new Product 
            { 
                Id = "P008", 
                Name = "External Hard Drive", 
                Price = 129.99m, 
                Description = "High-capacity external hard drive",
                StockQuantity = 80
            },
            new Product 
            { 
                Id = "P009", 
                Name = "E-Reader", 
                Price = 109.99m, 
                Description = "Lightweight e-reader for avid readers",
                StockQuantity = 70
            },
            new Product 
            { 
                Id = "P010", 
                Name = "Fitness Tracker", 
                Price =59.99m, 
                Description = "Advanced fitness tracker with heart rate monitor",
                StockQuantity = 200
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

        [HttpGet("routes")]
        public IActionResult GetAllRoutes()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Where(ad => ad.AttributeRouteInfo != null)
                .Select(ad => new 
                {
                    Method = ad.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? "Any",
                    Template = ad.AttributeRouteInfo.Template,
                    DisplayName = ad.DisplayName
                })
                .OrderBy(r => r.Template)
                .ToList();
            return Ok(routes);
        }
    }
} 