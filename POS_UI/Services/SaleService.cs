using System.Net.Http.Json;
using POS_UI.Models;
using Microsoft.Extensions.Logging;

namespace POS_UI.Services
{
    public class SaleService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SaleService> _logger;

        public SaleService(HttpClient httpClient, IConfiguration configuration, ILogger<SaleService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<Sale>> GetAllSalesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Sales");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned error status code: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error Response: {errorContent}");
                    return new List<Sale>();
                }

                var sales = await response.Content.ReadFromJsonAsync<List<Sale>>();
                return sales ?? new List<Sale>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching sales: {ex.Message}");
                return new List<Sale>();
            }
        }

        public async Task<Sale> GetSaleByIdAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Sale>($"api/Sales/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching sale {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Sales", sale);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned error status code: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error Response: {errorContent}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<Sale>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating sale: {ex.Message}");
                return null;
            }
        }
    }
} 