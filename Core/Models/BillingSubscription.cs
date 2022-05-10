using Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class BillingSubscription
    {
        public string SubscriptionId { get; set; }
        public string Plan { get; set; }
        public bool IsTrial { get; set; }
        public DateTime? TrialEnds { get; set; }
        public string Currency { get; set; }
        public decimal SubscriptionPrice { get; set; }
        public decimal AdditionalUserPrice { get; set; }
        public int NumberOfAdditionalUsers { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public decimal NextBillingAmount { get; set; }
    }
}