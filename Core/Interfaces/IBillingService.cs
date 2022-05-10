using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBillingService
    {
        Task<string> CreateSubscription(string planId, string customerId, string customerCompany, string customerEmail);

        Task<int> UpdateSubscriptionUsers(string subscriptionId, int quantity, string comment);

        Task<BillingSubscription> GetSubscriptionStatus(string subscriptionId);

        Dictionary<string, string> GetOptions();
    }
}
