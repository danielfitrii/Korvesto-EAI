using System.Net.Http.Json;
using Warehouse_UI.Models;
using Microsoft.Extensions.Logging;

namespace Warehouse_UI.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("POSApi");
            _configuration = configuration;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:PosApiUrl"]);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation($"Attempting to fetch products from: {_httpClient.BaseAddress}api/Products");
                var response = await _httpClient.GetAsync("api/Products");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch products. Status code: {response.StatusCode}");
                    return new List<Product>();
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to fetch product {id} from: {_httpClient.BaseAddress}api/Products/{id}");
                var response = await _httpClient.GetAsync($"api/Products/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch product. Status code: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var product = await response.Content.ReadFromJsonAsync<Product>();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product");
                return null;
            }
        }
    }
} 