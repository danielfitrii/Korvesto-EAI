using System.Net.Http.Json;
using POS_UI.Models;
using Microsoft.Extensions.Logging;

namespace POS_UI.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, IConfiguration configuration, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            _logger.LogInformation($"API Base URL: {baseUrl}");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to fetch products from API...");
                var response = await _httpClient.GetAsync("api/Products");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned error status code: {response.StatusCode}");
                    return new List<Product>();
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {content}");

                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                _logger.LogInformation($"Retrieved {products?.Count ?? 0} products");
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching products: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Product>($"api/Products/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching product {id}: {ex.Message}");
                return null;
            }
        }
    }
} 