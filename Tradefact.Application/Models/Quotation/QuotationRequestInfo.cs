using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models
{
    public class QuotationRequestInfo
    {
        public Guid Id { get; set; }
        public DateTime GoodsReady { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PortOfLoading { get; set; }
        public string PlaceOfDischarge { get; set; }
        public string PortOfDischarge { get; set; }
        public int Incoterms { get; set; }
        public int ShipmentType { get; set; }
        public int LoadType { get; set; }
        public string Reference { get; set; }
        public string Partner { get; set; }
        public List<PartnerQuoteInfo> Partners { get; set; }
        public string Client { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
        public int Revision { get; set; } = 0;
        public bool IsRevision => this.Revision > 1;
        public DateTime Submitted { get; set; }


        public int NoOfGoodsReadyDates { get; set; }
        public DateTime[] GoodsReadyDates { get; set; }
        public DateTime GoodsReadyMin { get; set; }
        public DateTime GoodsReadyMax { get; set; }

        public List<QuotationRequestScheduleInfo> Schedules { get; set; }
        [JsonIgnore]
        public Guid FreightMovementId { get; set; }

        public List<QuotationPurchaseOrderInfo> PurchaseOrderInfo { get; set; } = new List<QuotationPurchaseOrderInfo>();

    }

    public class QuotationRequestScheduleInfo
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid FreightMovementId { get; set; }
        public DateTime GoodsReady { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PlaceOfLoading { get; set; }
        public string ScheduleName { get; set; }

        [JsonIgnore]
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderReference { get; set; }
    }

    public class QuotationPurchaseOrderInfo
    {
        [JsonIgnore]
        public Guid QuotationRequestId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }

    public class PartnerQuoteInfo
    {
        public Guid QuotationId { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
    }
}
