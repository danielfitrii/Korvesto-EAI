using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Middleware_API.Models;
using Middleware_API.Services.Interfaces;
using Middleware_API.Tests.Helpers;
using Moq;
using Xunit;

namespace Middleware_API.Tests.Integration
{
    public class SaleControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IPOSService> _mockPosService;
        private readonly Mock<IWarehouseService> _mockWarehouseService;
        private readonly Mock<ICRMService> _mockCrmService;

        public SaleControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    _mockPosService = new Mock<IPOSService>();
                    _mockWarehouseService = new Mock<IWarehouseService>();
                    _mockCrmService = new Mock<ICRMService>();

                    services.AddScoped(_ => _mockPosService.Object);
                    services.AddScoped(_ => _mockWarehouseService.Object);
                    services.AddScoped(_ => _mockCrmService.Object);
                });
            });

            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-API-Key", "test-api-key");
        }

        [Fact]
        public async Task ProcessSale_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = TestDataHelper.CreateTestSaleRequest();
            var customer = TestDataHelper.CreateTestCustomer();
            var product = TestDataHelper.CreateTestProduct();
            var stock = TestDataHelper.CreateTestStock();
            var sale = TestDataHelper.CreateTestSale();

            _mockCrmService.Setup(x => x.GetCustomerAsync(request.CustomerId))
                .ReturnsAsync(customer);

            _mockPosService.Setup(x => x.GetProductAsync(request.ProductId))
                .ReturnsAsync(product);

            _mockWarehouseService.Setup(x => x.GetStockAsync(request.ProductId))
                .ReturnsAsync(stock);

            _mockPosService.Setup(x => x.CreateSaleAsync(It.IsAny<Sale>()))
                .ReturnsAsync(sale);

            _mockWarehouseService.Setup(x => x.AdjustStockAsync(It.IsAny<StockAdjustment>()))
                .ReturnsAsync(new Stock { ProductId = request.ProductId, Quantity = stock.Quantity - request.Quantity });

            _mockCrmService.Setup(x => x.UpdateLoyaltyPointsAsync(It.IsAny<string>(), It.IsAny<LoyaltyTransaction>()))
                .ReturnsAsync(new Customer { CustomerId = request.CustomerId, LoyaltyPoints = customer.LoyaltyPoints + 20 });

            // Act
            var response = await _client.PostAsJsonAsync("/api/sale/process-sale", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<SaleResponseDTO>();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(request.Quantity, result.SaleDetails.Quantity);
            Assert.Equal(product.Price * request.Quantity, result.SaleDetails.TotalAmount);
        }

        [Fact]
        public async Task GetSaleDetails_ValidId_ReturnsDetails()
        {
            // Arrange
            var sale = TestDataHelper.CreateTestSale();
            var product = TestDataHelper.CreateTestProduct();
            var customer = TestDataHelper.CreateTestCustomer();

            _mockPosService.Setup(x => x.GetSaleAsync(sale.Id))
                .ReturnsAsync(sale);

            _mockPosService.Setup(x => x.GetProductAsync(product.Id))
                .ReturnsAsync(product);

            _mockCrmService.Setup(x => x.GetCustomerAsync(customer.CustomerId))
                .ReturnsAsync(customer);

            // Act
            var response = await _client.GetAsync($"/api/sale/{sale.Id}/details");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<SaleDetailsResponseDTO>();
            Assert.NotNull(result);
            Assert.Equal(sale.Id, result.SaleId);
            Assert.Equal(customer.Name, result.Customer.Name);
            Assert.Equal(product.Name, result.Items[0].ProductName);
            Assert.Equal(customer.LoyaltyPoints, result.Loyalty.CurrentBalance);
        }

        [Fact]
        public async Task ProcessSale_InvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            var request = TestDataHelper.CreateTestSaleRequest(customerId: "InvalidCustomer");

            _mockCrmService.Setup(x => x.GetCustomerAsync("InvalidCustomer"))
                .ReturnsAsync((Customer)null);

            // Act
            var response = await _client.PostAsJsonAsync("/api/sale/process-sale", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<SaleResponseDTO>();
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("Customer", result.Errors[0]);
        }

        [Fact]
        public async Task ProcessSale_InvalidApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = TestDataHelper.CreateTestSaleRequest();

            // Act
            var response = await client.PostAsJsonAsync("/api/sale/process-sale", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProcessSale_InsufficientStock_ReturnsBadRequest()
        {
            // Arrange
            var request = TestDataHelper.CreateTestSaleRequest(quantity: 20);
            var customer = TestDataHelper.CreateTestCustomer();
            var product = TestDataHelper.CreateTestProduct();
            var stock = TestDataHelper.CreateTestStock(quantity: 10);

            _mockCrmService.Setup(x => x.GetCustomerAsync(request.CustomerId))
                .ReturnsAsync(customer);

            _mockPosService.Setup(x => x.GetProductAsync(request.ProductId))
                .ReturnsAsync(product);

            _mockWarehouseService.Setup(x => x.GetStockAsync(request.ProductId))
                .ReturnsAsync(stock);

            // Act
            var response = await _client.PostAsJsonAsync("/api/sale/process-sale", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<SaleResponseDTO>();
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("stock", result.Errors[0].ToLower());
        }
    }
} 