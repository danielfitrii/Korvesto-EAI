using Korvesto.Shared.Models;

namespace Middleware_API.Services.Interfaces
{
    public interface IPOSService
    {
        Task<Product> GetProductAsync(string productId);
        Task<Sale> CreateSaleAsync(Sale sale);
        Task<Sale> GetSaleAsync(string saleId);
    }
} 