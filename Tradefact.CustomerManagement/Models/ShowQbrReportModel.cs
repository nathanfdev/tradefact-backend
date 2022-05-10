using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Tradefact.Portal.Models
{
    public class ShowQbrReportModel
    {
        [JsonProperty("fromDate")]
        public DateTime FromDate;

        [JsonProperty("toDate")]
        public DateTime ToDate;

        [JsonProperty("organisationType")]
        public string OrganisationType;

        [JsonProperty("companyName")]
        public string CompanyName;

        [JsonProperty("contactName")]
        public string ContactName;

        [JsonProperty("contactEmail")]
        public string ContactEmail;

        [JsonProperty("contactPhone")]
        public string ContactTelephone;

        [JsonProperty("numActiveSuppliers")]
        public string NumActiveSuppliers;

        [JsonProperty("numActiveLogistics")]
        public string NumActiveLogistics;

        [JsonProperty("numPurchaseOrders")]
        public string NumPurchaseOrders;

        [JsonProperty("numDifferentSkusOrdered")]
        public string NumDifferentSKUsOrdered;

        [JsonProperty("numBookings")]
        public string NumBookings;

        [JsonProperty("avgTimeToShipDays")]
        public string AvgTimeToShipDays;

        [JsonProperty("avgTransitTimeDays")]
        public string AvgTransitTimeDays;

        [JsonProperty("avgPurchaseOrderToDeliveryDays")]
        public string AvgPurchaseOrderToDeliveryDays;

        [JsonProperty("avgTimeToQuoteDays")]
        public string AvgTimeToQuoteDays;

        [JsonProperty("quoteConversionRate")]
        public string QuoteConversionRate;

        [JsonProperty("avgFreightForwarderProfitMargin")]
        public string AvgFreightForwarderProfitMargin;

        [JsonProperty("numInvitationsSent")]
        public string NumInvitationsSent;

        [JsonProperty("numInvitationsAcceped")]
        public string NumInvitationsAccepted;
    }
}
