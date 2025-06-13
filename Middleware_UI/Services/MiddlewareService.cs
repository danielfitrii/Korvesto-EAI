using System.Net.Http.Json;
using Middleware_UI.Models;
using Microsoft.Extensions.Logging;

namespace Middleware_UI.Services
{
    public class MiddlewareService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MiddlewareService> _logger;

        public MiddlewareService(IHttpClientFactory httpClientFactory, ILogger<MiddlewareService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<ActivityLog>> GetActivityLogsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MiddlewareApi");
                _logger.LogInformation("Fetching activity logs from API");
                
                var response = await client.GetAsync("api/Middleware/logs");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch activity logs. Status code: {response.StatusCode}");
                    return new List<ActivityLog>();
                }

                var logs = await response.Content.ReadFromJsonAsync<List<ActivityLog>>();
                _logger.LogInformation($"Retrieved {logs?.Count ?? 0} activity logs");
                return logs ?? new List<ActivityLog>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching activity logs");
                return new List<ActivityLog>();
            }
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MiddlewareApi");
                _logger.LogInformation("Fetching products from API");
                
                var response = await client.GetAsync("api/Middleware/products");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch products. Status code: {response.StatusCode}");
                    return new List<Product>();
                }

                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                _logger.LogInformation($"Retrieved {products?.Count ?? 0} products");
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return new List<Product>();
            }
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MiddlewareApi");
                _logger.LogInformation("Fetching customers from CRM API");
                
                var response = await client.GetAsync("api/Middleware/customers"); // This will call Middleware API which in turn calls CRM API
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch customers. Status code: {response.StatusCode}");
                    return new List<Customer>();
                }

                var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
                _logger.LogInformation($"Retrieved {customers?.Count ?? 0} customers");
                return customers ?? new List<Customer>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                return new List<Customer>();
            }
        }

        public async Task<bool> ProcessSaleAsync(Sale sale)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MiddlewareApi");
                var response = await client.PostAsJsonAsync("api/Middleware/process-sale", sale);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sale");
                throw;
            }
        }
    }
} 