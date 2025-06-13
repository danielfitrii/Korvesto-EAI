using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace Middleware_UI.Pages
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<BasePageModel> _logger;

        protected BasePageModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<BasePageModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected HttpClient CreateAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient("MiddlewareAPI");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-API-Key", _configuration["ApiSettings:MiddlewareApiKey"]);
            return client;
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        protected IActionResult HandleApiError(Exception ex)
        {
            _logger.LogError(ex, "API request failed");
            SetErrorMessage("An error occurred while processing your request. Please try again later.");
            return Page();
        }
    }
} 