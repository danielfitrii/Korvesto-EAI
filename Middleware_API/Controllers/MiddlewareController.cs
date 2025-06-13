using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using Middleware_API.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Middleware_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiddlewareController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MiddlewareController> _logger;
        private static List<ActivityLog> _activityLogs = new();
        private static List<Product> _products = new()
        {
            new Product { Id = "P001", Name = "Laptop", Price = 999.99m },
            new Product { Id = "P002", Name = "Smartphone", Price = 499.99m },
            new Product { Id = "P003", Name = "Wireless Headphones", Price = 149.99m },
            new Product { Id = "P004", Name = "Tablet", Price = 299.99m },
            new Product { Id = "P005", Name = "Smartwatch", Price = 199.99m },
            new Product { Id = "P006", Name = "Gaming Console", Price = 399.99m },
            new Product { Id = "P007", Name = "Portable Speaker", Price = 79.99m },
            new Product { Id = "P008", Name = "External Hard Drive", Price = 129.99m },
            new Product { Id = "P009", Name = "E-Reader", Price = 109.99m },
            new Product { Id = "P010", Name = "Fitness Tracker", Price =59.99m }
        };

        public MiddlewareController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<MiddlewareController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            _logger.LogInformation("Fetching products");
            return Ok(_products);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var crmClient = _httpClientFactory.CreateClient("CRMApi");
                _logger.LogInformation("Middleware_API: Fetching customers from CRM API");

                var response = await crmClient.GetAsync("api/Customers");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Middleware_API: Failed to fetch customers from CRM API. Status: {response.StatusCode}, Content: {errorContent}");
                    return StatusCode((int)response.StatusCode, "Failed to fetch customers from CRM API");
                }

                var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
                _logger.LogInformation($"Middleware_API: Retrieved {customers?.Count ?? 0} customers from CRM API");
                return Ok(customers ?? new List<Customer>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Middleware_API: Error fetching customers from CRM API");
                return StatusCode(500, "An error occurred while fetching customers");
            }
        }

        [HttpPost("process-sale")]
        public async Task<IActionResult> ProcessSale([FromBody] Sale sale)
        {
            try
            {
                _logger.LogInformation($"Processing sale for customer: {sale.CustomerId}");

                // 1. Validate customer exists
                var crmClient = _httpClientFactory.CreateClient("CRMApi");
                var customerResponse = await crmClient.GetAsync($"api/Customers/{sale.CustomerId}");
                if (!customerResponse.IsSuccessStatusCode)
                {
                    return NotFound($"Customer {sale.CustomerId} not found");
                }

                // 2. Check product availability and update stock
                var warehouseClient = _httpClientFactory.CreateClient("WarehouseFlowApi");
                foreach (var item in sale.Items)
                {
                    _logger.LogInformation($"Middleware_API: Checking stock for ProductId: {item.ProductId}");
                    var stockResponse = await warehouseClient.GetAsync($"api/Stock/{item.ProductId}");
                    
                    if (!stockResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await stockResponse.Content.ReadAsStringAsync();
                        _logger.LogError($"Middleware_API: Failed to get stock for product {item.ProductId}. Status: {stockResponse.StatusCode}, Content: {errorContent}");
                        return BadRequest($"Product {item.ProductId} not found in stock or Warehouse API error: {errorContent}");
                    }

                    // Update stock
                    var stockAdjustment = new StockAdjustment
                    {
                        ProductId = item.ProductId,
                        Quantity = -item.Quantity, // Negative for reduction
                        Location = "Main Warehouse", // Add a default location
                        Reason = "Sale"
                    };

                    _logger.LogInformation($"Middleware_API: Attempting to adjust stock for ProductId: {stockAdjustment.ProductId}, Quantity: {stockAdjustment.Quantity}");
                    var adjustResponse = await warehouseClient.PostAsJsonAsync("api/Stock/adjust", stockAdjustment);
                    
                    if (!adjustResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await adjustResponse.Content.ReadAsStringAsync();
                        _logger.LogError($"Middleware_API: Failed to adjust stock for product {item.ProductId}. Status: {adjustResponse.StatusCode}, Content: {errorContent}");
                        return BadRequest($"Failed to update stock for product {item.ProductId}: {errorContent}");
                    }
                    _logger.LogInformation($"Middleware_API: Stock adjustment successful for ProductId: {item.ProductId}");
                }

                // 3. Record the sale
                var posClient = _httpClientFactory.CreateClient("StoreTrackApi");
                var saleResponse = await posClient.PostAsJsonAsync("api/Sales", sale);
                if (!saleResponse.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to record sale");
                }

                // 4. Update loyalty points
                var loyaltyTransaction = new LoyaltyTransaction
                {
                    CustomerId = sale.CustomerId,
                    Points = (int)(sale.TotalAmount * 10), // Example: 10 points per currency unit
                    Description = $"Points earned from sale {sale.Id}",
                    TransactionDate = DateTime.UtcNow
                };
                await crmClient.PostAsJsonAsync($"api/Loyalty/{sale.CustomerId}/points", loyaltyTransaction);

                // Log the activity
                _activityLogs.Add(new ActivityLog
                {
                    Timestamp = DateTime.UtcNow,
                    Operation = "ProcessSale",
                    Details = $"Sale processed for customer {sale.CustomerId}, total amount: {sale.TotalAmount}",
                    Status = "Success"
                });

                return Ok(new { Message = "Sale processed successfully", SaleId = sale.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sale");
                _activityLogs.Add(new ActivityLog
                {
                    Timestamp = DateTime.UtcNow,
                    Operation = "ProcessSale",
                    Details = $"Error processing sale: {ex.Message}",
                    Status = "Error"
                });
                return StatusCode(500, "An error occurred while processing the sale");
            }
        }

        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            return Ok(_activityLogs);
        }

        [HttpPost("logs")]
        public IActionResult AddLog([FromBody] ActivityLog log)
        {
            _activityLogs.Add(log);
            return Ok();
        }
    }

    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public required string Operation { get; set; }
        public required string Details { get; set; }
        public required string Status { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
} 