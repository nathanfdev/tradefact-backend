using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.FreightMovements.Queries.Model
{
    public partial class FreightMovementDTO
    {
        public Guid FreightMovementId { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        public DateTime? GoodsReady { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public LoadTypeEnum LoadType { get; set; }
        public decimal ConsignmentQuantity { get; set; }
        public bool InsuranceRequired { get; set; }
        public string InsuranceCurrency { get; set; }
        public decimal InsuranceValue { get; set; }
        public bool CustomsBrokerageRequired { get; set; }
        public int NumberOfItems { get; set; }
        public string TagsRaw { get; set; }
        public string HsCodesRaw { get; set; }
        public string Notes { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfLoading_Name { get; set; }
        public decimal? PortOfLoading_PositionLatitude { get; set; }
        public decimal? PortOfLoading_PositionLongitude { get; set; }
        public string PortOfLoading_CountryCode { get; set; }
        public string PortOfDischargeId { get; set; }
        public string PortOfDischarge_Name { get; set; }
        public decimal? PortOfDischarge_PositionLatitude { get; set; }
        public decimal? PortOfDischarge_PositionLongitude { get; set; }
        public string PortOfDischarge_CountryCode { get; set; }
        public Guid? PlaceOfDispatch_Id { get; set; }
        public string PlaceOfDispatch_Name { get; set; }
        public string PlaceOfDispatch_AddressLine1 { get; set; }
        public string PlaceOfDispatch_AddressLine2 { get; set; }
        public string PlaceOfDispatch_AddressLine3 { get; set; }
        public string PlaceOfDispatch_AddressLine4 { get; set; }
        public string PlaceOfDispatch_Province { get; set; }
        public string PlaceOfDispatch_PostalCode { get; set; }
        public string PlaceOfDispatch_City { get; set; }
        public string PlaceOfDispatch_County { get; set; }
        public string PlaceOfDispatch_CountryCode { get; set; }
        public string PlaceOfDispatch_Country { get; set; }
    }

    public partial class FreightMovementItemDTO
    {
        public Guid FreightMovementId { get; set; }
        public Guid FreightMovementItemId { get; set; }
        public string Description { get; set; }
        public string ContainerTypeCode { get; set; }
        
        public long IsPoSchedule { get; set; }
        public Guid ScheduleId { get; set; }
        public DateTime GoodsReady { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderReference { get; set; }
        public string PurchaseOrderTagsRaw { get; set; }
        public Guid? PlaceOfLoading_Id { get; set; }
        public string PlaceOfLoading_Name { get; set; }
        public string PlaceOfLoading_AddressLine1 { get; set; }
        public string PlaceOfLoading_AddressLine2 { get; set; }
        public string PlaceOfLoading_AddressLine3 { get; set; }
        public string PlaceOfLoading_AddressLine4 { get; set; }
        public string PlaceOfLoading_Province { get; set; }
        public string PlaceOfLoading_City { get; set; }
        public string PlaceOfLoading_PostalCode { get; set; }
        public string PlaceOfLoading_County { get; set; }
        public string PlaceOfLoading_CountryCode { get; set; }
        public string PlaceOfLoading_Country { get; set; }
        public double? PlaceOfLoading_Latitude { get; set; }
        public double? PlaceOfLoading_Longitude { get; set; }
    }

    public partial class CargoItemDTO
    {
        public Guid FreightMovementId { get; set; }
        public Guid FreightMovementItemId { get; set; }
        public Guid CargoItemId { get; set; }
        public string HsCode { get; set; }
        public string Sku { get; set; }
        public decimal Qty { get; set; }
        public int CartonQty { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public string UOL { get; set; }
        public decimal Weight { get; set; }
        public string UOW { get; set; }
        public bool IsProductVariant { get; set; }
        public Guid? Product_Id { get; set; }
        public string Product_Description { get; set; }
        public decimal Product_Dimensions_Height { get; set; }
        public decimal Product_Dimensions_Length { get; set; }
        public string Product_Dimensions_Scale { get; set; }
        public decimal Product_Dimensions_Width { get; set; }
        public string Product_GoodsType { get; set; }
        public string Product_HsCode { get; set; }
        public string Product_Name { get; set; }
        public string Product_Nickname { get; set; }
        public string Product_Packing { get; set; }
        public string Product_Sku { get; set; }
        public Guid? Product_CompanyId { get; set; }
        public string Product_HazardClass { get; set; }
        public string Product_HazardNotes { get; set; }
        public bool Product_MagneticFieldContained { get; set; }
        public int Product_UnitsPerPackage { get; set; }
        public int Product_HazardousContents { get; set; }
        public bool Product_Rotatable { get; set; }
        public bool Product_Stackable { get; set; }
        public bool ProductDescriptionOverride { get; set; }
        public long Product_Dimensions_Weight { get; set; }
        public string Product_Dimensions_WeightMeasurement { get; set; }
        public string Product_LithiumBatteryPacking { get; set; }
        public string Product_ThumbnailBlobUrl { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? PlaceOfLoadingId { get; set; }
        public string PlaceOfLoadingName { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
    }
}
