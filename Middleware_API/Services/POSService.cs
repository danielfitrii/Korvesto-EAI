using System.Net.Http.Json;
using Korvesto.Shared.Models;
using Middleware_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Middleware_API.Services
{
    public class POSService : IPOSService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<POSService> _logger;

        public POSService(IHttpClientFactory httpClientFactory, ILogger<POSService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("POSApi");
            _logger = logger;
        }

        public async Task<Product> GetProductAsync(string productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Products/{productId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Product {productId} not found. Status: {response.StatusCode}");
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product {productId}");
                throw;
            }
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Sales", sale);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create sale. Status: {response.StatusCode}, Error: {error}");
                    throw new Exception($"Failed to create sale: {error}");
                }
                return await response.Content.ReadFromJsonAsync<Sale>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                throw;
            }
        }

        public async Task<Sale> GetSaleAsync(string saleId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Sales/{saleId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Sale {saleId} not found. Status: {response.StatusCode}");
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<Sale>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching sale {saleId}");
                throw;
            }
        }
    }
} 