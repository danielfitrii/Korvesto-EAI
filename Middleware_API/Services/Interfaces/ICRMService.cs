using Korvesto.Shared.Models;

namespace Middleware_API.Services.Interfaces
{
    public interface ICRMService
    {
        Task<Customer> GetCustomerAsync(string customerId);
        Task<Customer> UpdateLoyaltyPointsAsync(string customerId, LoyaltyTransaction transaction);
    }
} 