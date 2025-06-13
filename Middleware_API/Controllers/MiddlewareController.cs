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
        private static List<ActivityLog> _activityLogs = new();

        public MiddlewareController(IHttpClientFactory httpClientFactory, ILogger<MiddlewareController> logger)
        {
            _posHttpClient = httpClientFactory.CreateClient("POSApi");
            _warehouseHttpClient = httpClientFactory.CreateClient("WarehouseApi");
            _crmHttpClient = httpClientFactory.CreateClient("CRMApi");
            _logger = logger;
        }

        [HttpGet("activity-logs")]
        public IActionResult GetActivityLogs()
        {
            return Ok(_activityLogs);
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
                    LogActivity("Error", "Failed to fetch sales", errorContent);
                    return StatusCode((int)response.StatusCode, $"POS API error: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var sales = JsonSerializer.Deserialize<List<Sale>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                LogActivity("Info", "Sales fetched", $"Retrieved {sales.Count} sales");
                return Ok(sales);
            }
            catch (Exception ex)
            {
                LogActivity("Error", "Error fetching sales", ex.Message);
                return StatusCode(500, $"Error: {ex.Message}");
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
                    LogActivity("Error", "Failed to fetch sales for processing", errorContent);
                    return StatusCode((int)salesResponse.StatusCode, $"POS API error: {errorContent}");
                }

                var salesJson = await salesResponse.Content.ReadAsStringAsync();
                var sales = JsonSerializer.Deserialize<List<Sale>>(salesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var results = new List<string>();
                foreach (var sale in sales)
                {
                    LogActivity("Info", "Processing sale", $"Sale ID: {sale.Id}, Customer: {sale.CustomerId}");

                    foreach (var item in sale.Items)
                    {
                        string url = $"api/stock/{item.ProductCode}/deduct/{item.Quantity}";
                        var deductResponse = await _warehouseHttpClient.PostAsync(url, null);

                        if (deductResponse.IsSuccessStatusCode)
                        {
                            var result = await deductResponse.Content.ReadAsStringAsync();
                            LogActivity("Info", "Stock deducted", $"Product: {item.ProductCode}, Quantity: {item.Quantity}, Result: {result}");
                            results.Add($" {item.ProductCode}: {item.Quantity} deducted → {result}");
                        }
                        else
                        {
                            var errorContent = await deductResponse.Content.ReadAsStringAsync();
                            LogActivity("Error", "Stock deduction failed", $"Product: {item.ProductCode}, Error: {errorContent}");
                            results.Add($" {item.ProductCode}: failed to deduct ({deductResponse.StatusCode}) - {errorContent}");
                        }
                    }

                    if (string.IsNullOrEmpty(sale.CustomerId))
                    {
                        LogActivity("Warning", "Loyalty points skipped", "No CustomerId provided");
                        results.Add($" Loyalty: Skipped - No CustomerId provided");
                        continue;
                    }

                    string crmUrl = $"api/customers/{sale.CustomerId}/add-points/10";
                    var crmResponse = await _crmHttpClient.PostAsync(crmUrl, null);

                    if (crmResponse.IsSuccessStatusCode)
                    {
                        var crmResult = await crmResponse.Content.ReadAsStringAsync();
                        LogActivity("Info", "Loyalty points added", $"Customer: {sale.CustomerId}, Points: +10, Result: {crmResult}");
                        results.Add($" Loyalty: +10 points for customer {sale.CustomerId} → {crmResult}");
                    }
                    else
                    {
                        var crmError = await crmResponse.Content.ReadAsStringAsync();
                        LogActivity("Error", "Loyalty points failed", $"Customer: {sale.CustomerId}, Error: {crmError}");
                        results.Add($" CRM: Failed to add points for customer {sale.CustomerId} ({crmResponse.StatusCode}) - {crmError}");
                    }
                }

                return Ok(new { message = "Processed sales and updated stock", details = results });
            }
            catch (Exception ex)
            {
                LogActivity("Error", "Processing error", ex.Message);
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private void LogActivity(string level, string action, string details)
        {
            var log = new ActivityLog
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Action = action,
                Details = details
            };
            _activityLogs.Add(log);
        }
    }

    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }  // Info, Warning, Error
        public string Action { get; set; } // What happened
        public string Details { get; set; } // Additional information
    }
}
