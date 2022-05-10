using Core.Common;
using Core.Enums;
using Core.Models;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.FreightMovements.Queries.Model;
using Tradefact.Application.Models;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Application.FreightMovements
{
    public class GetFreightMovementCargoItemsByIdQuery : IRequest<IPagedList<CargoItemResource>>
    {
        public Guid FreightMovementId { get; set; }

        [JsonIgnore]
        public Guid OrganisationId { get; set; }

        public Guid? ScheduleId { get; set; }

        public PagedResultParameters Paging { get; set; }
        public string Search { get; set; }

        public string ExistingFreightMovementItemId { get; set; }


        public class GetFreightMovementCargoItemsByIdQueryHandler : IRequestHandler<GetFreightMovementCargoItemsByIdQuery, IPagedList<CargoItemResource>>
        {
            private readonly ILogger<GetFreightMovementCargoItemsByIdQueryHandler> _logger;
            private readonly TradefactDbContext _context;
            private readonly IDbConnection _connection;

            // Using DI to inject infrastructure persistence Repositories
            public GetFreightMovementCargoItemsByIdQueryHandler(TradefactDbContext context, IDbConnection connection, ILogger<GetFreightMovementCargoItemsByIdQueryHandler> logger)
            {
                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<IPagedList<CargoItemResource>> Handle(GetFreightMovementCargoItemsByIdQuery request, System.Threading.CancellationToken cancellationToken)
            {
                string sQuery = @$"
                                    SELECT  FM.Id [FreightMovementId], FM.Name, FM.Reference, FM.TransactionType, FM.ShipmentType, FM.IncoTerms, FM.GoodsReady, FM.DeliveryDate, FM.LoadType, FM.ConsignmentQuantity,
                                            FM.InsuranceRequired, FM.InsuranceCurrency, FM.InsuranceValue, FM.CustomsBrokerageRequired, FM.NumberOfItems, FM.Tags [TagsRaw], FM.HSCodes [HSCodesRaw],
                                            FM.Notes, 
                                            FM.PortOfLoadingId, PL.Name [PortOfLoading_Name], PL.Position_Latitude [PortOfLoading_Position_Latitude], PL.Position_Longitude [PortOfLoading_Position_Longitude], PL.CountryCode [PortOfLoading_CountryCode],
                                            FM.PortOfDischargeId, PD.Name [PortOfDischarge_Name], PD.Position_Latitude [PortOfDischarge_Position_Latitude], PD.Position_Longitude [PortOfDischarge_Position_Longitude], PD.CountryCode [PortOfDischarge_CountryCode],
                                            AD.Id [PlaceOfDispatch_Id], AD.Name [PlaceOfDispatch_Name], AD.AddressLine1 [PlaceOfDispatch_AddressLine1], AD.AddressLine2 [PlaceOfDispatch_AddressLine2], AD.AddressLine3 [PlaceOfDispatch_AddressLine3], AD.AddressLine4 [PlaceOfDispatch_AddressLine4], AD.PostalCode [PlaceOfDispatch_PostalCode], 
                                            AD.Province [PlaceOfDispatch_Province], AD.City [PlaceOfDispatch_City], AD.County [PlaceOfDispatch_County], AD.CountryCode [PlaceOfDispatch_CountryCode], C.Name [PlaceOfDispatch_Country]
                                     FROM [dbo].[FreightMovements] FM 
                                        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = FM.PortOfLoadingId
                                        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = FM.PortOfDischargeId
                                        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                        LEFT JOIN [dbo].[Countries] C ON C.Code2 = AD.CountryCode
                                    WHERE FM.Id = @FreightMovementId

                                    SELECT  FMI.FreightMovementId, FMI.Id [FreightMovementItemId], 
                                            ISNULL(FMI.Schedule, FMI.ContainerType) [Description],
                                            CASE WHEN FMI.Schedule IS NOT NULL THEN 1 ELSE 0 END [IsPOSchedule] ,
                                            ISNULL(FMI.ConfirmedGoodsReadyDate, FMI.GoodsReady) [GoodsReady], FMI.PlaceOfLoadingId, 
                                            FMI.PurchaseOrderId, PO.PurchaseOrderNumber, PO.Reference [PurchaseOrderReference], PO.Tags [PurchaseOrderTagsRaw],
                                            AD.Id [PlaceOfLoading_Id], AD.Name [PlaceOfLoading_Name], AD.AddressLine1 [PlaceOfLoading_AddressLine1], AD.AddressLine2 [PlaceOfLoading_AddressLine2], AD.AddressLine3 [PlaceOfLoading_AddressLine3], AD.AddressLine4 [PlaceOfLoading_AddressLine4], AD.PostalCode [PlaceOfLoading_PostalCode],
                                            AD.Province [PlaceOfLoading_Province], AD.City [PlaceOfLoading_City], AD.County [PlaceOfLoading_County], AD.Position_Latitude  [PlaceOfLoading_Latitude], AD.Position_Longitude [PlaceOfLoading_Longitude], AD.CountryCode [PlaceOfLoading_CountryCode], C.Name [PlaceOfLoading_Country]
                                     FROM (
                                        SELECT FMI.FreightMovementId, FMI.Id, PSL.ConfirmedGoodsReadyDate, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], FM.GoodsReady, FMI.ContainerTypeCode, CT.[Description] [ContainerType], PSL.[Description] [Schedule],
                                        PSL.PurchaseOrderId, PSL.Id [ScheduleId]
                                        FROM [dbo].[FreightMovements] FM
                                            INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                            LEFT JOIN [dbo].ContainerTypes CT ON CT.Code = FMI.ContainerTypeCode
                                            LEFT JOIN  [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                        WHERE FM.Id = @FreightMovementId
                                        GROUP BY FMI.FreightMovementId, FMI.Id, PSL.[Description], PSL.ConfirmedGoodsReadyDate, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), FM.GoodsReady, FMI.ContainerTypeCode, CT.[Description], PSL.PurchaseOrderId, PSL.Id
                                    ) FMI
                                        INNER JOIN  [dbo].[FreightMovements] FM ON FM.Id = FMI.FreightMovementId
                                        LEFT JOIN  [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId
                                        LEFT JOIN  [dbo].[Addresses] AD ON AD.Id = ISNULL(FMI.PlaceOfLoadingId, FM.PlaceOfLoadingId)
                                        LEFT JOIN [dbo].[Countries] C ON C.Code2 = AD.CountryCode

                                    ;WITH cte_fmi AS (
                                        SELECT FMI.FreightMovementId, FMI.Id, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], PSL.PurchaseOrderId
                                        FROM [dbo].[FreightMovements] FM
                                            INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                            LEFT JOIN  [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                        WHERE FM.Id = @FreightMovementId
                                        GROUP BY FMI.FreightMovementId, FMI.Id, ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), PSL.PurchaseOrderId
                                    )
                                    SELECT  CI.FreightMovementId, CI.FreightMovementItemId, CI.ItemId [CargoItemId], 
                                        ISNULL(P.HsCode, PV.HsCode) [HsCode],
                                        ISNULL(P.SKU, PV.SKU) [SKU],
                                        FMI.PurchaseOrderId,
                                        PO.PurchaseOrderNumber,
                                        FMI.PlaceOfLoadingId,
                                        AD.Name [PlaceOfLoadingName],
                                        PO.SupplierId,
                                        S.Name [SupplierName],
                                        CI.Qty,
                                        CI.CartonQty,
                                        CI.[Width], 
                                        CI.[Length], 
                                        CI.[Height], 
                                        CI.[UOL],    
                                        CI.[Weight], 
                                        CI.[UOW], 
                                        CI.IsProductVariant,
                                        CI.ProductDescriptionOverride,
                                        ISNULL(P.Id,PV.Id) [Product_Id],
                                        ISNULL(CI.ItemDescription, ISNULL(P.Description,PV.Description)) [Product_Description],
                                        ISNULL(P.Dimensions_Height,PV.Dimensions_Height) [Product_Dimensions_Height],
                                        ISNULL(P.Dimensions_Length,PV.Dimensions_Length) [Product_Dimensions_Length],
                                        ISNULL(P.Dimensions_Scale,PV.Dimensions_Scale) [Product_Dimensions_Scale],
                                        ISNULL(P.Dimensions_Width,PV.Dimensions_Width) [Product_Dimensions_Width],
                                        ISNULL(P.Dimensions_Weight,PV.Dimensions_Weight) [Product_Dimensions_Weight],
                                        ISNULL(P.Dimensions_weightMeasurement,PV.Dimensions_weightMeasurement) [Product_Dimensions_WeightMeasurement],
                                        ISNULL(P.GoodsType,PV.GoodsType) [Product_GoodsType],
                                        ISNULL(P.HsCode,PV.HsCode) [Product_HsCode],
                                        ISNULL(CI.ItemDescription, P.Name) [Product_Name],
                                        ISNULL(P.Nickname,PV.Nickname) [Product_Nickname],
                                        ISNULL(P.Packing,PV.Packing) [Product_Packing],
                                        ISNULL(P.SKU,PV.SKU) [Product_SKU],
                                        ISNULL(P.HazardClass,PV.HazardClass) [Product_HazardClass],
                                        ISNULL(P.HazardDocumentId,PV.HazardDocumentId) [Product_HazardDocumentId],
                                        ISNULL(P.HazardNotes,PV.HazardNotes) [Product_HazardNotes],
                                        ISNULL(P.MagneticFieldContained,PV.MagneticFieldContained) [Product_MagneticFieldContained],
                                        ISNULL(P.Reference,PV.Reference) [Product_Reference],
                                        ISNULL(P.HazardDescription,PV.HazardDescription) [Product_HazardDescription],
                                        ISNULL(P.UnitsPerPackage,PV.UnitsPerPackage) [Product_UnitsPerPackage],
                                        ISNULL(P.HazardousContents,PV.HazardousContents) [Product_HazardousContents],
                                        P.Rotatable [Product_Rotatable],
                                        P.Stackable [Product_Stackable],
                                        CASE
                                        WHEN D.ThumbnailGenerated = CAST(1 AS bit) THEN D.ThumbnailUrl
                                        ELSE D.BlobUrl END AS [Product_ThumbnailBlobUrl],
                                        ISNULL(P.Dimensions_Weight,PV.Dimensions_Weight) [Product_Dimensions_Weight],
                                        ISNULL(P.Dimensions_weightMeasurement,PV.Dimensions_weightMeasurement) [Product_Dimensions_weightMeasurement],
                                        P.LithiumBatteryPacking [Product_LithiumBatteryPacking]
                                     FROM [dbo].[CargoItems] CI 
                                        LEFT JOIN [dbo].[Products] P ON P.Id = CI.ProductId
                                        LEFT JOIN [dbo].[ProductVariants] PV ON PV.Id = CI.ProductId
                                        LEFT JOIN [dbo].[ProductDocuments] PD ON P.Id = PD.ProductId AND PD.IsActive = 1 AND PD.IsDefaultImage = 1
                                        LEFT JOIN [dbo].[Documents] D ON D.Id = PD.DocumentId  AND D.Active = 1
                                        LEFT JOIN cte_fmi FMI ON FMI.Id = CI.FreightMovementItemId
                                        LEFT JOIN FreightMovements FM ON FM.Id = FMI.FreightMovementId
                                        LEFT JOIN  [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId
                                        LEFT JOIN  [dbo].[Addresses] AD ON AD.Id = ISNULL(FMI.PlaceOfLoadingId, FM.PlaceOfLoadingId)
                                        LEFT JOIN [dbo].[Organisations] S ON S.Id = PO.SupplierId
                                    WHERE CI.FreightMovementId = @FreightMovementId
                                ";

                if (request.ScheduleId.HasValue)
                {
                    sQuery = sQuery + $" AND CI.PurchaseOrderItemScheduleLineId = @ScheduleId";
                }


                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @FreightMovementId = request.FreightMovementId,
                        @PageSize = request.Paging.PageSize,
                        @PageNumber = request.Paging.PageNumber,
                        @ScheduleId = request.ScheduleId
                    }, commandTimeout: 60))
                    {
                        FreightMovementDTO freight_movement = await multi.ReadSingleAsync<FreightMovementDTO>();
                        List<FreightMovementItemDTO> freight_movement_items = (await multi.ReadAsync<FreightMovementItemDTO>()).ToList();

                        List<CargoItemDTO> freight_movement_cargo_items = null;
                        if (!string.IsNullOrEmpty(request.Search))
                        {
                            freight_movement_cargo_items = (await multi.ReadAsync<CargoItemDTO>())
                                .Where(q => EF.Functions.Like(q.Product_Name, $"%{request.Search}%") 
                                || EF.Functions.Like(q.Product_Description, $"%{request.Search}%")
                                || EF.Functions.Like(q.Product_Sku, $"%{request.Search}%")).ToList();
                        }
                        else
                        {
                            freight_movement_cargo_items = (await multi.ReadAsync<CargoItemDTO>()).ToList();
                        }
                        if (request.ExistingFreightMovementItemId != null)
                        {
                            Guid freightMovItemId = new Guid(request.ExistingFreightMovementItemId);
                            freight_movement_cargo_items = freight_movement_cargo_items.Where(q=>q.FreightMovementItemId == freightMovItemId).ToList();
                        }                        
                        IPagedList<CargoItemResource> cargoItems = await freight_movement_cargo_items.Adapt<List<CargoItemResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);

                        return cargoItems;
                    }
                }
            }
        }
    }
}

