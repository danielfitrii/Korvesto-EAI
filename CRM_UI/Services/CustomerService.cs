using System.Net.Http.Json;
using CRM_UI.Models;
using Microsoft.Extensions.Logging;

namespace CRM_UI.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CustomerService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CRMApi");
            _configuration = configuration;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:CRMApiUrl"]);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            try
            {
                _logger.LogInformation($"Attempting to fetch customers from: {_httpClient.BaseAddress}api/Customers");
                var response = await _httpClient.GetAsync("api/Customers");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch customers. Status code: {response.StatusCode}");
                    return new List<Customer>();
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
                return customers ?? new List<Customer>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                return new List<Customer>();
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to fetch customer {id} from: {_httpClient.BaseAddress}api/Customers/{id}");
                var response = await _httpClient.GetAsync($"api/Customers/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch customer. Status code: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var customer = await response.Content.ReadFromJsonAsync<Customer>();
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer");
                return null;
            }
        }

        public async Task<Customer?> CreateCustomerAsync(Customer customer)
        {
            try
            {
                _logger.LogInformation($"Attempting to create customer: {customer.CustomerId}");
                var response = await _httpClient.PostAsJsonAsync("api/Customers", customer);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to create customer. Status code: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {content}");

                var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();
                return createdCustomer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return null;
            }
        }
    }
} 