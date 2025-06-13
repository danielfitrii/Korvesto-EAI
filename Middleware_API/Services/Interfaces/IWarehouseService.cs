using Korvesto.Shared.Models;

namespace Middleware_API.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<Stock> GetStockAsync(string productId);
        Task<Stock> AdjustStockAsync(StockAdjustment adjustment);
    }
} 