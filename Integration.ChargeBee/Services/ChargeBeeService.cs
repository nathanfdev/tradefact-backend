using ChargeBee.Api;
using ChargeBee.Models;
using ChargeBee.Models.Enums;
using Core.Caching;
using Core.Common.Caching;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Integration.ChargeBee
{
    public class ChargeBeeService : IBillingService
    {
        private class ProductCatalogue
        {
            public List<ListResult.Entry> Plans;
            public List<ListResult.Entry> Addons;
        }

        private readonly ILogger<ChargeBeeService> _logger;
        private readonly ChargeBeeOptions _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ProductCatalogue _productCatalogue;

        public ChargeBeeService(ILogger<ChargeBeeService> logger, ChargeBeeOptions options, IMemoryCache memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            ApiConfig.Configure(options.SiteId, options.APIKey);

            // Cache list of subscription plans and addons
            var cacheKey = CacheKey.With(GetType(), $"TF_Chargebee_Product_Catalogue");
            _productCatalogue = _memoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                cacheEntry = new MemoryCacheEntryOptions 
                { 
                    AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddMinutes(60)) 
                };

                return new ProductCatalogue
                {
                    Plans = Plan.List().Status().Is(Plan.StatusEnum.Active).Request().List,
                    Addons = Addon.List().Status().Is(Addon.StatusEnum.Active).Request().List
                };
            });
        }

        public Dictionary<string, string> GetOptions()
        {
            return (Dictionary<string, string>)_options.AsDictionary();
        }

        public async Task<string> CreateSubscription(string planId, string customerId, string customerCompany, string customerEmail)
        {
            return await Task.Run(() =>
            {
                if (_productCatalogue.Plans.Count(p => p.Plan.Id == planId) == 0)
                {
                    throw new KeyNotFoundException($"{planId} plan is not set up in Chargebee");
                }

                // Create Chargebee subscription
                EntityResult result = Subscription.Create()
                    .PlanId(planId)
                    .CustomerId(customerId)
                    .CustomerCompany(customerCompany)
                    .CustomerEmail(customerEmail)
                    .Request();

                return result.Subscription.Id;
            });
        }

        public async Task<int> UpdateSubscriptionUsers(string subscriptionId, int quantity, string comment)
        {
            return await Task.Run(() =>
            {
                EntityResult result = Subscription.Retrieve(subscriptionId).Request();

                var addonId = result.Subscription.PlanId + _options.userAddonSuffix;

                if (_productCatalogue.Addons.Count(p => p.Addon.Id == addonId) > 0)
                {
                    if (quantity == 0)
                    {
                        result = Subscription.Update(subscriptionId)
                            .ReplaceAddonList(true)
                            .EndOfTerm(false)
                            .Request();
                    } 
                    else 
                    { 
                        result = Subscription.Update(subscriptionId)
                            .AddonId(0, addonId)
                            .AddonQuantity(0, quantity)
                            .EndOfTerm(false)   // Set true to apply at the end of the current billing cycle instead of immediately
                            .Request();
                    }

                    _ = Comment.Create()
                        .EntityId(subscriptionId)
                        .EntityType(EntityTypeEnum.Subscription)
                        .Notes(comment)
                        .Request();
                }

                return result.Subscription.Addons?.Count(a => a.Id() == addonId) ?? 0;
            });
        }

        private decimal getAdditionalUserPrice(Subscription subscription)
        {
            var addonId = subscription.PlanId + _options.userAddonSuffix;
            var addon = _productCatalogue.Addons.FirstOrDefault(a => a.Addon.Id.Equals(addonId))?.Addon;
            return (addon?.Price ?? 0) / 100;
        }

        public async Task<BillingSubscription> GetSubscriptionStatus(string subscriptionId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    EntityResult result = Subscription.Retrieve(subscriptionId).Request();
                    Subscription subscription = result.Subscription;

                    var addonId = subscription.PlanId + _options.userAddonSuffix;

                    return new BillingSubscription
                    {
                        SubscriptionId = subscription.Id,
                        Plan = subscription.PlanId,
                        IsTrial = subscription.Status == Subscription.StatusEnum.InTrial,
                        TrialEnds = subscription.TrialEnd,
                        Currency = subscription.CurrencyCode,
                        SubscriptionPrice = (subscription.PlanAmount ?? 0) / 100,
                        AdditionalUserPrice = getAdditionalUserPrice(subscription),
                        NumberOfAdditionalUsers = result.Subscription.Addons?.Count(a => a.Id() == addonId) ?? 0,
                        NextBillingDate = subscription.NextBillingAt,
                        NextBillingAmount = (subscription.TotalDues ?? 0) / 100
                    };
                }
                catch
                {
                    return new BillingSubscription
                    {
                        SubscriptionId = "Subscription not found",
                        Plan = "None",
                        IsTrial = false
                    };
                }
            });
        }

    }
}
