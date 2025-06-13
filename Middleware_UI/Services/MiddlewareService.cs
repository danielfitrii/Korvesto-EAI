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