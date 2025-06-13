using System.Net.Http.Json;
using Korvesto.Shared.Models;
using Middleware_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Middleware_API.Services
{
    public class CRMService : ICRMService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CRMService> _logger;

        public CRMService(IHttpClientFactory httpClientFactory, ILogger<CRMService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CRMApi");
            _logger = logger;
        }

        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Customers/{customerId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Customer {customerId} not found. Status: {response.StatusCode}");
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<Customer>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching customer {customerId}");
                throw;
            }
        }

        public async Task<Customer> UpdateLoyaltyPointsAsync(string customerId, LoyaltyTransaction transaction)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Loyalty/{customerId}/points", transaction);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to update loyalty points. Status: {response.StatusCode}, Error: {error}");
                    throw new Exception($"Failed to update loyalty points: {error}");
                }
                return await response.Content.ReadFromJsonAsync<Customer>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loyalty points");
                throw;
            }
        }
    }
} 