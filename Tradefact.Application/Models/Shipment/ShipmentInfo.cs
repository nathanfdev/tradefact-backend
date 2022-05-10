using Core.Attributes;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models.Shipment;
using Tradefact.Application.Shipments;

namespace Tradefact.Application.Models
{
    [TypescriptAutoGeneration]
    public class ShipmentTransitInfo
    {
        public Guid Id { get; set; }
        public string SCAC { get; set; }
        public string IMO { get; set; }
        public string VesselName { get; set; }
    }

    [TypescriptAutoGeneration]
    public class GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


    public class CoreExceptionsInfo
    {
        public int ShipmentType { get; set; }
        [JsonIgnore]
        public bool Delivered { get; set; }
        [JsonIgnore]
        public DateTime? EstimatedCollectionDate { get; set; }
        [JsonIgnore]
        public bool Collected { get; set; }
        public DateTime? EstimatedDeparturePOL { get; set; }
        public DateTime? EstimatedArrivalPOD { get; set; }
        public bool DepartedPOL { get; set; }
        public bool ArrivedPOD { get; set; }
        [JsonIgnore]
        public bool IssueAtCustoms { get; set; }
        [JsonIgnore]
        public bool CustomsClearence { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }

    public class ExceptionsInfo : CoreExceptionsInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public List<string> Exceptions => new ShipmentExceptionsInfo().GetShipmentExceptions(this);
        [JsonIgnore]
        public bool HasExceptions => (this.Exceptions.Count > 0);
    }

    public class ShipmentMapInfo : CoreExceptionsInfo
    {
        public Guid Id { get; set; }
        public string Partner { get; set; }
        public string Reference { get; set; }
        public List<string> Exceptions => new ShipmentExceptionsInfo().GetShipmentExceptions(this);
        public bool HasExceptions => (this.Exceptions.Count > 0);
        public string PlaceOfLoading { get; set; }
        public string PortOfLoadingCode { get; set; }
        public string PortOfLoading { get; set; }
        public string PlaceOfDischarge { get; set; }
        public string PortOfDischarge { get; set; }
        public string PortOfDischargeCode { get; set; }
        public GeoCoordinate PortOfLoadingGeoCoordinate => (this.PortOfLoading_Latitude.HasValue && this.PortOfLoading_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PortOfLoading_Latitude.GetValueOrDefault(), Longitude = this.PortOfLoading_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PortOfDischargeGeoCoordinate => (this.PortofDischarge_Latitude.HasValue && this.PortofDischarge_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PortofDischarge_Latitude.GetValueOrDefault(), Longitude = this.PortofDischarge_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PlaceOfLoadingGeoCoordinate => (this.PlaceOfLoading_Latitude.HasValue && this.PlaceOfLoading_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PlaceOfLoading_Latitude.GetValueOrDefault(), Longitude = this.PlaceOfLoading_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PlaceOfDischargeGeoCoordinate => (this.PlaceOfDischarge_Latitude.HasValue && PlaceOfDischarge_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PlaceOfDischarge_Latitude.GetValueOrDefault(), Longitude = this.PlaceOfDischarge_Longitude.GetValueOrDefault() } : null;

        [JsonIgnore]
        public double? PortofDischarge_Latitude { get; set; }
        [JsonIgnore]
        public double? PortofDischarge_Longitude { get; set; }
        [JsonIgnore]
        public double? PortOfLoading_Latitude { get; set; }
        [JsonIgnore]
        public double? PortOfLoading_Longitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfDischarge_Latitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfDischarge_Longitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfLoading_Latitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfLoading_Longitude { get; set; }
        public Guid FreightMovementId { get; set; }
        public FreightMovementResource FreightMovement { get; set; }
        public bool MultiplePickupLocations => this.FreightMovement?.NoLoadingLocations > 1;
    }


    [TypescriptAutoGeneration]
    public class ShipmentInfo : CoreExceptionsInfo
    {
        public Guid Id { get; set; }
        public DateTime GoodsReady { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PortOfLoadingCode { get; set; }
        public string PortOfLoading { get; set; }
        public string PlaceOfDischarge { get; set; }
        public string PortOfDischarge { get; set; }
        public string PortOfDischargeCode { get; set; }
        public int Incoterms { get; set; }
        public int LoadType { get; set; }
        public int TransactionType { get; set; }
        public ShipmentStatus Status { get; set; }

        public GeoCoordinate PortOfLoadingGeoCoordinate => (this.PortOfLoading_Latitude.HasValue && this.PortOfLoading_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PortOfLoading_Latitude.GetValueOrDefault(), Longitude = this.PortOfLoading_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PortOfDischargeGeoCoordinate => (this.PortofDischarge_Latitude.HasValue && this.PortofDischarge_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PortofDischarge_Latitude.GetValueOrDefault(), Longitude = this.PortofDischarge_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PlaceOfLoadingGeoCoordinate => (this.PlaceOfLoading_Latitude.HasValue && this.PlaceOfLoading_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PlaceOfLoading_Latitude.GetValueOrDefault(), Longitude = this.PlaceOfLoading_Longitude.GetValueOrDefault() } : null;

        public GeoCoordinate PlaceOfDischargeGeoCoordinate => (this.PlaceOfDischarge_Latitude.HasValue && PlaceOfDischarge_Longitude.HasValue) ? new GeoCoordinate { Latitude = this.PlaceOfDischarge_Latitude.GetValueOrDefault(), Longitude = this.PlaceOfDischarge_Longitude.GetValueOrDefault() } : null;

        [JsonIgnore]
        public double? PortofDischarge_Latitude { get; set; }
        [JsonIgnore]
        public double? PortofDischarge_Longitude { get; set; }
        [JsonIgnore]
        public double? PortOfLoading_Latitude { get; set; }
        [JsonIgnore]
        public double? PortOfLoading_Longitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfDischarge_Latitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfDischarge_Longitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfLoading_Latitude { get; set; }
        [JsonIgnore]
        public double? PlaceOfLoading_Longitude { get; set; }


        [JsonIgnore]
        public ShipmentStage Stage { get; set; }

        public string Reference { get; set; }
        public string Partner { get; set; }
        public string Client { get; set; }
        public string Supplier { get; set; }
        public string Name { get; set; }

        // Tracking 
        public string SCAC { get; set; }
        public string BillofLadingNumber { get; set; }
        public string IMO { get; set; }
        public string VesselName { get; set; }

        [JsonIgnore]
        public string Latitude { get; set; }
        [JsonIgnore]
        public string Longitude { get; set; }

        public int State { get; set; }
        public DateTime Submitted { get; set; }
        public DateTime? CollectionDate { get; set; }

        [JsonIgnore]
        public bool Booked { get; set; }
        [JsonIgnore]
        public DateTime? BookedDate { get; set; }
        public DateTime? ArrivedPODDate { get; set; }
        public DateTime? DepartedPOLDate { get; set; }

        [JsonIgnore]
        public bool InTransit { get; set; }
        [JsonIgnore]
        public DateTime? InTransitDate { get; set; }

        [JsonIgnore]
        public bool EquipmentTrackAvailable { get; set; }
        [JsonIgnore]
        public bool ShipmentTrackAvailable { get; set; }

        [JsonIgnore]
        public bool InCustoms { get; set; }
        [JsonIgnore]
        public DateTime? IssueAtCustomsDate { get; set; }
        [JsonIgnore]
        public bool IssueAtCustomCleared { get; set; }
        [JsonIgnore]
        public DateTime? CustomsClearenceDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [JsonIgnore]
        public bool TrackinformationAdded { get; set; }

        [JsonIgnore]
        public string Tags { get; set; }

        [JsonIgnore]
        public string ContainerList { get; set; }

        public bool IsRescheduled { get; set; }
        public int Rescheduled { get; set; }
        public DateTime? LastRescheduleTime { get; set; }

        [JsonIgnore]
        public string Route { get; set; }

        [JsonProperty("equipment")]
        public List<string> Equipment => (this.ContainerList == null) ? new List<string>() : this.ContainerList.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        [JsonProperty("tags")]
        public List<string> TagsList => (this.Tags == null) ? new List<string>() : this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        [JsonProperty("route")]
        public RouteSchedule Schedule => (this.Route == null) ? null : JsonConvert.DeserializeObject<RouteSchedule>(this.Route);

        [JsonProperty("stage")]
        public ShipmentStages ShipmentStage { get; set; }

        [JsonIgnore]
        public bool HasExceptions => (this.Exceptions.Count > 0);

        public List<String> Exceptions => new ShipmentExceptionsInfo().GetShipmentExceptions(this);

        public ShipmentActions AvailableActions => this.GetShipmentActions();
        public TimeLine Timeline => this.GetShipmentTimeLine();

        public Guid FreightMovementId { get; set; }
        public Guid QuotationRequestId { get; set; }
        public FreightMovementResource FreightMovement { get; set; }
        public QuotationResource Quotation { get; set; }

        public List<TrackingResource> BillTracking { get; set; } = new List<TrackingResource>();

        public int NoOfDocuments { get; set; }
        
        public int NoOfGoodsReadyDates { get; set; }
        public DateTime[] GoodsReadyDates { get; set; }
        public DateTime GoodsReadyMin { get; set; }
        public DateTime GoodsReadyMax { get; set; }

        public List<ShipmentScheduleInfo> Schedules { get; set; } = new List<ShipmentScheduleInfo>();
        public bool BillTrackingAvailable => this.EquipmentTrackAvailable;

        public bool MultiplePickupLocations => this.FreightMovement?.NoLoadingLocations > 1;
        public bool MultipleDropLocations => false;

        public List<string> PurchaseOrderNumbers { get; set; } = new List<string>();
        public List<ShipmentPurchaseOrderInfo> PurchaseOrderInfo { get; set; } = new List<ShipmentPurchaseOrderInfo>();
        public List<ShipmentLocationInfo> LoadingLocations { get; set; } = new List<ShipmentLocationInfo>();

        public int NoOfLoadingLocations => this.LoadingLocations.Count;

        public string PackingListURL { get; set; }
        public bool PackingListReady { get; set; }

        private ShipmentActions GetShipmentActions()
        {
            ShipmentActions actions = new ShipmentActions();

            if (this.Delivered)
            {
                return actions;
            }

            actions.AddSchedule = !(this.EstimatedDeparturePOL.HasValue && this.EstimatedDeparturePOL.GetValueOrDefault() > new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            ShipmentStateMachine sm = new ShipmentStateMachine(this.Status, this.Stage);

            actions.SetCollectionDate = this.Booked & !this.Collected & !this.EstimatedCollectionDate.HasValue;
            actions.EditCollectionDate = this.Booked & !this.Collected & this.EstimatedCollectionDate.HasValue;

            actions.RecordCollected = this.Booked & !this.Collected & this.EstimatedCollectionDate.HasValue;

            if (this.ShipmentType == 1)
            {
                actions.AddTrackingInfo = this.Collected && !this.TrackinformationAdded && !this.ArrivedPOD;
                actions.EditTrackingInfo = this.Collected && this.TrackinformationAdded && !this.ArrivedPOD;
            }

            if (this.ShipmentType == 2)
            {
                actions.AddBolNumber = this.Collected && !this.TrackinformationAdded && !this.ArrivedPOD;
                actions.EditBolNumber = this.Collected && this.TrackinformationAdded && !this.ArrivedPOD;
            }

            actions.ConfirmDepartedPOL = this.Collected && !this.DepartedPOL && (this.EstimatedDeparturePOL < DateTime.Now);
            actions.ConfirmArrivedPOD = !this.ArrivedPOD && (this.DepartedPOL && this.EstimatedArrivalPOD < DateTime.Now);

            // Reschedule available at any point up to departed
            actions.Reschedule = !actions.AddSchedule && !this.DepartedPOL;

            actions.RecordIssueAtCustoms = this.ArrivedPOD && !this.IssueAtCustoms && !this.CustomsClearence;
            actions.ClearCustoms = this.ArrivedPOD && !this.CustomsClearence;

            if (this.CustomsClearence && !this.Delivered)
            {
                actions.SetEstimatedDeliveryDate = !this.EstimatedDeliveryDate.HasValue;
                actions.EditEstimatedDeliveryDate = this.EstimatedDeliveryDate.HasValue;
                actions.MarkDelivered = this.EstimatedDeliveryDate.HasValue;
            }

            return actions;
        }

        private TimeLine GetShipmentTimeLine()
        {
            TimeLine t = new TimeLine();
            if (this.Booked)
            {
                t.Booked.State = Core.Types.EventClassifier.Complete;
                t.Booked.DateTime = this.BookedDate;
                if (this.EstimatedCollectionDate.HasValue)
                {
                    t.Booked.EstimatedCollection = this.EstimatedCollectionDate.GetValueOrDefault();
                }
            }

            t.Collection.State = Core.Types.EventClassifier.Active;
            t.Collection.DateTime = this.EstimatedCollectionDate;
            if (this.Collected)
            {
                t.Collection.State = Core.Types.EventClassifier.Complete;
                t.Collection.DateTime = this.BookedDate;
            }

            if (this.InTransit || this.Collected)
            {
                t.InTransit.State = Core.Types.EventClassifier.Active;
                t.InTransit.DateTime = this.CollectionDate.GetValueOrDefault();

                t.InTransit.InTransitToPOL = this.Collected && !this.DepartedPOL;
                t.InTransit.Shipping = this.DepartedPOL && !this.ArrivedPOD;
                t.InTransit.ArrivedPOD = this.ArrivedPOD;

                if (this.Delivered || this.ArrivedPOD || this.CustomsClearence || this.IssueAtCustoms)
                {
                    t.InTransit.State = Core.Types.EventClassifier.Complete;
                    t.InTransit.DateTime = this.ArrivedPODDate;
                }
            }

            if (t.InTransit.State == Core.Types.EventClassifier.Complete)
            {
                t.Customs.State = Core.Types.EventClassifier.Active;
                t.Customs.IssueAtCustoms = this.IssueAtCustoms && !this.CustomsClearence;
                if (this.CustomsClearence)
                {
                    t.Customs.State = Core.Types.EventClassifier.Complete;
                    t.Customs.DateTime = this.CustomsClearenceDate;
                }
            }

            if (this.CustomsClearence)
            {
                t.Delivery.State = Core.Types.EventClassifier.Active;
                if (this.Delivered)
                {
                    t.Delivery.State = Core.Types.EventClassifier.Complete;
                    t.Delivery.DateTime = this.DeliveryDate;
                }
            }

            return t;
        }
    }

    [TypescriptAutoGeneration]
    public class ShipmentPurchaseOrderInfo
    {
        [JsonIgnore]
        public Guid ShipmentId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }

    [TypescriptAutoGeneration]
    public class ShipmentScheduleInfo
    {
        [JsonIgnore]
        public Guid ShipmentId { get; set; }
        [JsonIgnore]
        public Guid FreightMovementId { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderReference { get; set; }

        public DateTime GoodsReady { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PlaceOfLoading { get; set; }
        public string ScheduleName { get; set; }

        [JsonIgnore]
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        public AddressResource Address { get; set; }
    }

    [TypescriptAutoGeneration]
    public class ShipmentLocationInfo
    {
        public string LocationId { get; set; }
        public string SupplierName { get; set; }
        public AddressResource Address { get; set; }
    }


    public class ShipmentScheduleInfoDTO: ShipmentScheduleInfo
    {
        public Guid AddressId { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }



    [TypescriptAutoGeneration]
    public class ShipmentActions
    {
        [JsonProperty(Order = 1)]
        public bool EditCollectionDate { get; set; } = false;
        [JsonProperty(Order = 1)]
        public bool SetCollectionDate { get; set; } = false;
        [JsonProperty(Order = 2)]
        public bool RecordCollected { get; set; } = false;

        [JsonProperty(Order = 3)]
        public bool AddTrackingInfo { get; set; } = false;

        [JsonProperty(Order = 4)]
        public bool EditTrackingInfo { get; set; } = false;

        [JsonProperty(Order = 5)]
        public bool ConfirmDepartedPOL { get; set; } = false;  // Only available when estimated date departure reached

        [JsonProperty(Order = 6)]
        public bool ConfirmArrivedPOD { get; set; } = false;

        [JsonProperty(Order = 7)]
        public bool RecordIssueAtCustoms { get; set; } = false;

        [JsonProperty(Order = 8)]
        public bool ClearCustoms { get; set; } = false;

        [JsonProperty(Order = 9)]
        public bool SetEstimatedDeliveryDate { get; set; } = false;

        [JsonProperty(Order = 10)]
        public bool EditEstimatedDeliveryDate { get; set; } = false;

        [JsonProperty(Order = 11)]
        public bool MarkDelivered { get; set; } = false;

        [JsonProperty(Order = 12)]
        public bool Suspend { get; set; } = false;

        [JsonProperty(Order = 13)]
        public bool Terminate { get; set; } = false;

        [JsonProperty(Order = 14)]
        public bool Reactivate { get; set; } = false;

        [JsonProperty(Order = 15)]
        public bool Complete { get; set; } = false;

        [JsonProperty(Order = 15)]
        public bool Archive { get; set; } = false;

        [JsonProperty(Order = 16)]
        public bool AddBolNumber { get; set; } = false;

        [JsonProperty(Order = 17)]
        public bool EditBolNumber { get; set; } = false;

        [JsonProperty(Order = 18)]
        public bool AddSchedule { get; set; } = false;

        [JsonProperty(Order = 19)]
        public bool Reschedule { get; set; } = false;

    }

    [TypescriptAutoGeneration]
    public class ShipmentStages
    {
        public bool Booked { get; set; } = false;
        // Waiting for Collection
        public bool AwaitingCollectionDate { get; set; } = false;      // Add Collection Date
        public bool AwaitingCollection { get; set; } = false;          // Ready for collection
        // Shipping
        public bool AwaitingTrackingInformation { get; set; } = false; // IMO | MMSI & BOL | Container Information
        public bool InTransitToPort { get; set; } = false;             // Collection Confirmed (Via Management | API data)
        public bool Shipping { get; set; } = false;
        public bool DeparturePOLConfirmed { get; set; } = false;       // Departure Confirmed (Via Management | API data)
        public bool ArrivalPODConfirmed { get; set; } = false;         // Arrival Confirmed (Via Management | API data)
                                                                       // Customs
        public bool PendingCustoms { get; set; } = false;              // Pending Customs Clearance
        public bool IssueAtCustoms { get; set; } = false;              // Issue At Customs
        public bool InTransitToDestination { get; set; } = false;      // Customs Cleared
        public bool Delivered { get; set; } = false;
    }

    public class ShipmentPackingListInfo
    {
        public ShipmentInfo Shipment { get; set; }
        public List<ShipmentLegInfo> Legs { get; set; }
        public List<ShipmentProductInfo> Products { get; set; }

    }

    public class ShipmentLegInfo: Address
    {
        public int Seq { get; set; }
        public bool IsPort { get; set; }
        public string Description { get; set; }

        public IEnumerable<string> NoBlankAddress() {

            Func<string, bool> hasValue = str => !string.IsNullOrEmpty(str) && str.Length > 0;
            if (hasValue(this.AddressLine1 ?? "")) yield return this.AddressLine1;
            if (hasValue(this.AddressLine2 ?? "")) yield return this.AddressLine2;
            if (hasValue(this.AddressLine3 ?? "")) yield return this.AddressLine3;
            if (hasValue(this.AddressLine4 ?? "")) yield return this.AddressLine4;
            if (hasValue(this.City ?? "")) yield return this.City;
            if (hasValue(this.PostalCode ?? "")) yield return this.PostalCode;

            yield break;
        }
    }

    public class ShipmentProductInfo
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ShipmentId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        
        public string Product_SKU { get; set; }
        public string Product_Description { get; set; }
        public decimal QtyCommitted { get; set; }
        public List<string> PurchaseOrders { get; set; }
    }

}