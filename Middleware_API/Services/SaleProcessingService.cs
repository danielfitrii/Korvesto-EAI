using Korvesto.Shared.Models;
using Middleware_API.Models;
using Middleware_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Middleware_API.Services
{
    public class SaleProcessingService
    {
        private readonly IPOSService _posService;
        private readonly IWarehouseService _warehouseService;
        private readonly ICRMService _crmService;
        private readonly ILogger<SaleProcessingService> _logger;
        private static readonly List<TransactionLog> _transactionLogs = new();

        public SaleProcessingService(
            IPOSService posService,
            IWarehouseService warehouseService,
            ICRMService crmService,
            ILogger<SaleProcessingService> logger)
        {
            _posService = posService;
            _warehouseService = warehouseService;
            _crmService = crmService;
            _logger = logger;
        }

        public async Task<SaleResponseDTO> ProcessSaleAsync(SaleRequestDTO request)
        {
            var transactionId = Guid.NewGuid().ToString();
            var response = new SaleResponseDTO
            {
                TransactionId = transactionId,
                Success = false
            };

            try
            {
                LogTransaction(transactionId, "Sale Processing", "Started", "Processing new sale request");

                // 1. Validate customer
                var customer = await _crmService.GetCustomerAsync(request.CustomerId);
                if (customer == null)
                {
                    var error = $"Customer {request.CustomerId} not found";
                    LogTransaction(transactionId, "Customer Validation", "Failed", error);
                    response.Errors.Add(error);
                    return response;
                }
                LogTransaction(transactionId, "Customer Validation", "Success", $"Customer {request.CustomerId} validated");

                // 2. Validate product
                var product = await _posService.GetProductAsync(request.ProductId);
                if (product == null)
                {
                    var error = $"Product {request.ProductId} not found";
                    LogTransaction(transactionId, "Product Validation", "Failed", error);
                    response.Errors.Add(error);
                    return response;
                }
                LogTransaction(transactionId, "Product Validation", "Success", $"Product {request.ProductId} validated");

                // 3. Check stock availability
                var stock = await _warehouseService.GetStockAsync(request.ProductId);
                if (stock == null || stock.Quantity < request.Quantity)
                {
                    var error = $"Insufficient stock for product {request.ProductId}";
                    LogTransaction(transactionId, "Stock Check", "Failed", error);
                    response.Errors.Add(error);
                    return response;
                }
                LogTransaction(transactionId, "Stock Check", "Success", $"Stock available for product {request.ProductId}");

                // 4. Create sale record
                var sale = new Sale
                {
                    Id = transactionId,
                    CustomerId = request.CustomerId,
                    SaleDate = DateTime.UtcNow,
                    Items = new List<SaleItem>
                    {
                        new SaleItem
                        {
                            ProductId = request.ProductId,
                            Quantity = request.Quantity,
                            UnitPrice = product.Price,
                            TotalPrice = product.Price * request.Quantity
                        }
                    },
                    TotalAmount = product.Price * request.Quantity
                };

                var recordedSale = await _posService.CreateSaleAsync(sale);
                LogTransaction(transactionId, "Sale Creation", "Success", $"Sale recorded with ID {recordedSale.Id}");

                // 5. Adjust stock
                var stockAdjustment = new StockAdjustment
                {
                    ProductId = request.ProductId,
                    Quantity = -request.Quantity,
                    Location = "POS",
                    Reason = $"Sale {recordedSale.Id}"
                };
                await _warehouseService.AdjustStockAsync(stockAdjustment);
                LogTransaction(transactionId, "Stock Adjustment", "Success", $"Stock adjusted for product {request.ProductId}");

                // 6. Update loyalty points
                var loyaltyTransaction = new LoyaltyTransaction
                {
                    CustomerId = request.CustomerId,
                    Points = (int)(sale.TotalAmount / 10),
                    Description = $"Earned from sale {recordedSale.Id}",
                    TransactionDate = DateTime.UtcNow
                };
                await _crmService.UpdateLoyaltyPointsAsync(request.CustomerId, loyaltyTransaction);
                LogTransaction(transactionId, "Loyalty Update", "Success", $"Loyalty points updated for customer {request.CustomerId}");

                // 7. Prepare success response
                response.Success = true;
                response.Message = "Sale processed successfully";
                response.SaleDetails = new SaleDetailsDTO
                {
                    SaleId = recordedSale.Id,
                    CustomerId = recordedSale.CustomerId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    TotalAmount = recordedSale.TotalAmount,
                    LoyaltyPointsEarned = loyaltyTransaction.Points,
                    TransactionDate = recordedSale.SaleDate
                };

                LogTransaction(transactionId, "Sale Processing", "Completed", "Sale processed successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sale");
                LogTransaction(transactionId, "Sale Processing", "Failed", $"Error: {ex.Message}");
                response.Errors.Add($"Error processing sale: {ex.Message}");
                return response;
            }
        }

        public async Task<SaleDetailsResponseDTO> GetSaleDetailsAsync(string saleId)
        {
            try
            {
                // 1. Get sale details from POS
                var sale = await _posService.GetSaleAsync(saleId);
                if (sale == null)
                {
                    throw new Exception($"Sale {saleId} not found");
                }

                // 2. Get customer details from CRM
                var customer = await _crmService.GetCustomerAsync(sale.CustomerId);
                if (customer == null)
                {
                    throw new Exception($"Customer {sale.CustomerId} not found");
                }

                // 3. Get product details for each item
                var items = new List<SaleItemDetailsDTO>();
                foreach (var item in sale.Items)
                {
                    var product = await _posService.GetProductAsync(item.ProductId);
                    items.Add(new SaleItemDetailsDTO
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Unknown Product",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    });
                }

                // 4. Get loyalty details
                var loyaltyTransaction = new LoyaltyTransaction
                {
                    CustomerId = sale.CustomerId,
                    Points = (int)(sale.TotalAmount / 10),
                    Description = $"Earned from sale {sale.Id}",
                    TransactionDate = sale.SaleDate
                };

                return new SaleDetailsResponseDTO
                {
                    SaleId = sale.Id,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount,
                    Items = items,
                    Customer = new CustomerDetailsDTO
                    {
                        CustomerId = customer.CustomerId,
                        Name = customer.Name,
                        Email = customer.Email,
                        Phone = customer.Phone
                    },
                    Loyalty = new LoyaltyDetailsDTO
                    {
                        PointsEarned = loyaltyTransaction.Points,
                        CurrentBalance = customer.LoyaltyPoints,
                        TransactionDescription = loyaltyTransaction.Description
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting sale details for {saleId}");
                throw;
            }
        }

        private void LogTransaction(string transactionId, string operation, string status, string details)
        {
            var log = new TransactionLog
            {
                Id = Guid.NewGuid().ToString(),
                TransactionId = transactionId,
                Operation = operation,
                Status = status,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _transactionLogs.Add(log);
            _logger.LogInformation($"Transaction {transactionId}: {operation} - {status} - {details}");
        }

        public IEnumerable<TransactionLog> GetTransactionLogs(string transactionId)
        {
            return _transactionLogs.Where(log => log.TransactionId == transactionId)
                                 .OrderBy(log => log.Timestamp);
        }
    }
} 