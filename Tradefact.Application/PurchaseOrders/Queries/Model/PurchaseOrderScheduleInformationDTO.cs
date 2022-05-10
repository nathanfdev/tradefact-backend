using Core.Models;
using System;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class PurchaseOrderScheduleInformationDTO
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid ScheduleId { get; set; }
        public DateTime? ConfirmedGoodsReadyDate { get; set; }
        public string Description { get; set; }
        public int ItemQty { get; set; }
        public string Supplier { get; set; }
        public string CountryofLoadingCode { get; set; }
        public string Country { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PlaceOfLoading_Address1 { get; set; }
        public string PlaceOfLoading_Address2 { get; set; }
        public string PlaceOfLoading_Address3 { get; set; }
        public string PlaceOfLoading_Address4 { get; set; }
        public string PlaceOfLoading_City { get; set; }
        public string CurrencyId { get; set; }
        public decimal LineValue { get; set; }
    }

    public class PurchaseOrderItemScheduleInformationDTO : PurchaseOrderScheduleInformationDTO
    {
        public Guid PurchaseOrderItemId { get; set; }
    }
}