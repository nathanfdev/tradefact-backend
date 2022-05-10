using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public partial class PurchaseOrderItemDTO
    {
        public Guid PurchaseOrderId { get; set; }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Guid MasterProductId { get; set; }

        public bool IsProductVariant { get; set; }

        public decimal OrderQuantity { get; set; }

        public decimal ScheduleLineCommittedQuantity { get; set; }

        public decimal RemainingQuantity { get; set; }

        public decimal OrderPriceUnit { get; set; }
        public string OrderQuantityUnit { get; set; }
        public string SupplierReference { get; set; }
        public decimal Amount { get; set; }

        public string Product_Description { get; set; }

        public decimal Product_DimensionsHeight { get; set; }

        public decimal Product_DimensionsLength { get; set; }

        public string Product_DimensionsScale { get; set; }

        public decimal Product_DimensionsWidth { get; set; }

        public string Product_GoodsType { get; set; }

        public string Product_HsCode { get; set; }

        public string Product_Name { get; set; }

        public string Product_Nickname { get; set; }

        public string Product_Packing { get; set; }

        public string Product_Sku { get; set; }

        public Guid Product_CompanyId { get; set; }

        public string Product_HazardClass { get; set; }

        public string Product_HazardNotes { get; set; }

        public bool Product_MagneticFieldContained { get; set; }

        public int Product_UnitsPerPackage { get; set; }

        public int Product_HazardousContents { get; set; }

        public bool Product_Rotatable { get; set; }

        public bool Product_Stackable { get; set; }
        public string Product_ThumbnailBlobUrl { get; set; }
        public bool ProductDescriptionOverride { get; set; }

        public long Product_DimensionsWeight { get; set; }

        public string Product_DimensionsWeightMeasurement { get; set; }

        public string Product_LithiumBatteryPacking { get; set; }

        public string Product_Tags { get; set; }

        public List<PurchaseOrderItemScheduleInformationDTO> Schedules { get; set; }
        public List<PurchaseOrderItemShipmentInformationDTO> Shipments { get; set; }

        public OrderShipmentStatusEnum OrderShipmentStatus => this.GetOrderShipmentStatus();

        private OrderShipmentStatusEnum GetOrderShipmentStatus()
        {
            if (this.Shipments != null && this.Shipments.Count > 0)
            {
                int shipments_count = this.Shipments.Count;

                if (this.Shipments.Count(q => q.Booked) == shipments_count) return OrderShipmentStatusEnum.ALL_BOOKED;

                if (this.Shipments.Count(q => q.Booked || q.InTransit || q.Delivered) > 0) return OrderShipmentStatusEnum.BOOKED_READY_PENDING;

                return OrderShipmentStatusEnum.READY_PENDING_NOT_BOOKED;
            }
            return OrderShipmentStatusEnum.NONE_PENDING_READY_BOOKED;
        }

    }

}
