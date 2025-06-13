using System.Net.Http.Json;
using Warehouse_UI.Models;
using Microsoft.Extensions.Logging;

namespace Warehouse_UI.Services
{
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StockService> _logger;

        public StockService(HttpClient httpClient, IConfiguration configuration, ILogger<StockService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<List<StockItem>> GetAllStockAsync()
        {
            try
            {
                _logger.LogInformation($"Attempting to fetch stock from: {_httpClient.BaseAddress}api/Stock");
                var response = await _httpClient.GetAsync("api/Stock");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch stock. Status code: {response.StatusCode}");
                    return new List<StockItem>();
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var stockList = await response.Content.ReadFromJsonAsync<List<StockItem>>();
                return stockList ?? new List<StockItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock");
                return new List<StockItem>();
            }
        }

        public async Task<StockItem> AdjustStockAsync(StockAdjustment adjustment)
        {
            try
            {
                _logger.LogInformation($"Attempting to adjust stock for product {adjustment.ProductId}");
                var response = await _httpClient.PostAsJsonAsync("api/Stock/adjust", adjustment);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to adjust stock. Status code: {response.StatusCode}, Error: {error}");
                    throw new Exception($"Failed to adjust stock: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<StockItem>();
                if (result == null)
                {
                    throw new Exception("Failed to deserialize stock adjustment response");
                }

                _logger.LogInformation($"Successfully adjusted stock for product {adjustment.ProductId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                throw;
            }
        }
    }
} 