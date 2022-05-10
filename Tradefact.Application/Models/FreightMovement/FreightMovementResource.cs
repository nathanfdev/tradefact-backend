using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.Models
{
    public class TrackingResource
    {
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        [JsonProperty("container")]
        public string ContainerNo { get; set; }

        [JsonProperty("lastEvent")]
        public string LastEvent { get; set; }

        [JsonProperty("lastEventTime")]
        public string LastEventTime { get; set; }

        [JsonProperty("trackingEvents")]
        public List<TrackingEventResource> TrackingEventResource { get; set; }
    }
    public class TrackingEventResource
    {
        [JsonProperty("timeOfEvent")]
        public DateTimeOffset TimeOfEvent { get; set; }

        [JsonProperty("location")]
        public string LocationOfEvent { get; set; }

        [JsonProperty("voyage")]
        public string Voyage { get; set; }

        [JsonProperty("activity")]
        public string Activity { get; set; }

        [JsonProperty("Information")]
        public string Information { get; set; }
    }

    public class FreightMovementResource
    {
        public Guid FreightMovementId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Reference { get; set; }

        // BASIC
        public int? TransactionType { get; set; }
        public int? ShipmentMethod { get; set; }
        public int? IncoTerms { get; set; }

        // ORIGIN
        public DateTime GoodsReady { get; set; }
        public DateTime? GoodsReadyMin => this.Items.Min(X => X.GoodsReady);
        public DateTime? GoodsReadyMax => this.Items.Min(X => X.GoodsReady);

        public int NoOfGoodsReadyDates { get; set; }
        public DateTime[] GoodsReadyDates { get; set; }

        // DESTINATION
        public DateTime? DeliveryDate { get; set; }

        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PlaceOfDispatch { get; set; }
        public int NoLoadingLocations { get; set; }

        public Guid? PurchaseOrderId { get; set; }

        public Location PortOfLoadingInfo { get; set; }
        public Location PortOfDischargeInfo { get; set; }
        public AddressResource PlaceOfLoadingInfo { get; set; }
        public AddressResource PlaceOfDispatchInfo { get; set; }

        // CARGO
        public int? LoadType { get; set; }

        public decimal ConsignmentQuantity { get; set; }

        public List<FCLItemResource> FCL { get; set; }
        public List<LCLItemResource> LCL { get; set; }

        public List<FreightMovementItemResource> Items { get; set; } = new List<FreightMovementItemResource>();


        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public int? NumberOfItems { get; set; } = 1;


        public List<string> Tags { get; set; }
        public List<string> HsCodes { get; set; }
        public string Notes { get; set; }
        public List<FreightMovementResourceScheduleInfo> Schedules { get; set; }
        public List<GeographicPosition> LoadingGeoMarkers { get; set; } = new List<GeographicPosition>();
        public List<GeographicPosition> DispatchGeoMarkers { get; set; } = new List<GeographicPosition>();
    }

    public class FreightMovementResourceScheduleInfo
    {
        public DateTime GoodsReady { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PlaceOfLoading { get; set; }
        public string ScheduleName { get; set; }
    }

    public class FreightMovementItemResource
    {
        public Guid FreightMovementItemId { get; set; }
        public string Description { get; set; }
        public string ContainerTypeCode { get; set; }

        public bool IsPOSchedule { get; set; }
        public Guid ScheduleId { get; set; }
        public DateTime? GoodsReady { get; set; }
        public string PlaceOfLoadingId { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderReference { get; set; }
        public string PurchaseOrderTags { get; set; }

        public AddressResource PlaceOfLoadingInfo { get; set; }
        public List<CargoItemResource> CargoItems { get; set; }
    }


}
