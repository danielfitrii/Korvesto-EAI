using System.Net.Http.Json;
using Korvesto.Shared.Models;
using Middleware_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Middleware_API.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WarehouseService> _logger;

        public WarehouseService(IHttpClientFactory httpClientFactory, ILogger<WarehouseService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("WarehouseApi");
            _logger = logger;
        }

        public async Task<Stock> GetStockAsync(string productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Stock/{productId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Stock for product {productId} not found. Status: {response.StatusCode}");
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<Stock>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching stock for product {productId}");
                throw;
            }
        }

        public async Task<Stock> AdjustStockAsync(StockAdjustment adjustment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Stock/adjust", adjustment);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to adjust stock. Status: {response.StatusCode}, Error: {error}");
                    throw new Exception($"Failed to adjust stock: {error}");
                }
                return await response.Content.ReadFromJsonAsync<Stock>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                throw;
            }
        }
    }
} 