using Microsoft.AspNetCore.Mvc;
using Korvesto.Shared.Models;
using System.Net.Http;
using System.Text.Json;

namespace Middleware_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiddlewareController : ControllerBase
    {
        private readonly HttpClient _posHttpClient;
        private readonly HttpClient _warehouseHttpClient;
        private readonly HttpClient _crmHttpClient;
        private readonly ILogger<MiddlewareController> _logger;

        public MiddlewareController(IHttpClientFactory httpClientFactory, ILogger<MiddlewareController> logger)
        {
            _posHttpClient = httpClientFactory.CreateClient("POSApi");
            _warehouseHttpClient = httpClientFactory.CreateClient("WarehouseApi");
            _crmHttpClient = httpClientFactory.CreateClient("CRMApi");
            _logger = logger;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesFromPOS()
        {
            try
            {
                var response = await _posHttpClient.GetAsync("api/sales");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"POS API returned {response.StatusCode}: {errorContent}");
                    return StatusCode((int)response.StatusCode, $"POS API error: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var sales = JsonSerializer.Deserialize<List<Sale>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(sales);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to POS API");
                return StatusCode(500, $"Error connecting to POS API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching sales");
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("process-sales")]
        public async Task<IActionResult> ProcessSalesAndUpdateStock()
        {
            try
            {
                var salesResponse = await _posHttpClient.GetAsync("api/sales");

                if (!salesResponse.IsSuccessStatusCode)
                {
                    var errorContent = await salesResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"POS API returned {salesResponse.StatusCode}: {errorContent}");
                    return StatusCode((int)salesResponse.StatusCode, $"POS API error during sales fetch: {errorContent}");
                }

                var salesJson = await salesResponse.Content.ReadAsStringAsync();
                var sales = JsonSerializer.Deserialize<List<Sale>>(salesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var results = new List<string>();
                foreach (var sale in sales)
                {
                    foreach (var item in sale.Items)
                    {
                        string url = $"api/stock/{item.ProductCode}/deduct/{item.Quantity}";
                        var deductResponse = await _warehouseHttpClient.PostAsync(url, null);

                        if (deductResponse.IsSuccessStatusCode)
                        {
                            var result = await deductResponse.Content.ReadAsStringAsync();
                            results.Add($" {item.ProductCode}: {item.Quantity} deducted → {result}");
                        }
                        else
                        {
                            var errorContent = await deductResponse.Content.ReadAsStringAsync();
                            results.Add($" {item.ProductCode}: failed to deduct ({deductResponse.StatusCode}) - {errorContent}");
                        }
                    }

                    // Reward customer: +10 points per sale
                    string crmUrl = $"api/customers/{sale.CustomerName}/add-points/10";
                    var crmResponse = await _crmHttpClient.PostAsync(crmUrl, null);

                    if (crmResponse.IsSuccessStatusCode)
                    {
                        var crmResult = await crmResponse.Content.ReadAsStringAsync();
                        results.Add($" Loyalty: +10 points → {crmResult}");
                    }
                    else
                    {
                        var crmError = await crmResponse.Content.ReadAsStringAsync();
                        results.Add($" CRM: Failed to add points ({crmResponse.StatusCode}) - {crmError}");
                    }
                }

                return Ok(new { message = "Processed sales and updated stock", details = results });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to one of the APIs during sales processing");
                return StatusCode(500, $"Error connecting to API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during sales processing");
                return StatusCode(500, $"Unexpected error during processing: {ex.Message}");
            }
        }
    }
}
