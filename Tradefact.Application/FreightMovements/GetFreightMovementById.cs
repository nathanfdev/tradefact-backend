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

namespace Tradefact.Application.FreightMovements
{
    public class GetFreightMovementByIdQuery : IRequest<FreightMovementResource>
    {
        public Guid FreightMovementId { get; set; }

        [JsonIgnore]
        public Guid OrganisationId { get; set; }


        public class GetFreightMovementByIdQueryHandler : IRequestHandler<GetFreightMovementByIdQuery, FreightMovementResource>
        {
            private readonly ILogger<GetFreightMovementByIdQueryHandler> _logger;
            private readonly TradefactDbContext _context;
            private readonly IDbConnection _connection;

            // Using DI to inject infrastructure persistence Repositories
            public GetFreightMovementByIdQueryHandler(TradefactDbContext context, IDbConnection connection, ILogger<GetFreightMovementByIdQueryHandler> logger)
            {
                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<FreightMovementResource> Handle(GetFreightMovementByIdQuery request, System.Threading.CancellationToken cancellationToken)
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

                                    SELECT  CI.FreightMovementId, CI.FreightMovementItemId, CI.ItemId [CargoItemId], 
                                        ISNULL(P.HsCode, PV.HsCode) [HsCode],
                                        ISNULL(P.SKU, PV.SKU) [SKU],
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
                                    WHERE CI.FreightMovementId = @FreightMovementId

                                ";

                FreightMovementResource fm;
                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        @FreightMovementId = request.FreightMovementId
                    }, commandTimeout: 60))
                    {
                        FreightMovementDTO freight_movement = await multi.ReadSingleAsync<FreightMovementDTO>();

                        List<FreightMovementItemDTO> freight_movement_items = (await multi.ReadAsync<FreightMovementItemDTO>()).ToList();

                        List<CargoItemDTO> freight_movement_cargo_items = (await multi.ReadAsync<CargoItemDTO>()).ToList();

                        fm = freight_movement.Adapt<FreightMovementResource>();

                        List<AddressResource> list_loading_addresses = new List<AddressResource>();

                        foreach (var freight_movement_item in freight_movement_items)
                        {
                            FreightMovementItemResource fg = freight_movement_item.Adapt<FreightMovementItemResource>();

                            if (freight_movement_item.PlaceOfLoading_Id.HasValue)
                            {
                                if (!list_loading_addresses.Any(q=>q.Id == freight_movement_item.PlaceOfLoading_Id.GetValueOrDefault().ToString()))
                                {
                                    list_loading_addresses.Add(fg.PlaceOfLoadingInfo);
                                }
                            }

                            fg.CargoItems = freight_movement_cargo_items
                                    .Where(q => q.FreightMovementId == freight_movement_item.FreightMovementId && q.FreightMovementItemId == freight_movement_item.FreightMovementItemId)
                                    .Select(s=> s.Adapt<CargoItemResource>()).ToList();

                            fm.Items.Add(fg);
                        }

                        fm.NoLoadingLocations = list_loading_addresses.Count;
                        if (fm.NoLoadingLocations == 1)
                        {
                            fm.PlaceOfLoadingInfo = list_loading_addresses[0];
                        }

                        fm.LoadingGeoMarkers.AddRange(list_loading_addresses.Where(q => q.Position.Latitude.HasValue && q.Position.Longitude.HasValue).Select(s=>s.Position));

                        fm.GoodsReadyDates = fm.Items.OrderBy(o => o.GoodsReady.GetValueOrDefault()).Select(s => s.GoodsReady.GetValueOrDefault()).Distinct().ToArray();
                        fm.NoOfGoodsReadyDates = fm.GoodsReadyDates.Count();

                        LoadTypeEnum loadtype = (LoadTypeEnum)fm.LoadType.GetValueOrDefault();
                        if (loadtype <= LoadTypeEnum.PENDING) loadtype = LoadTypeEnum.PENDING;

                        if (loadtype == LoadTypeEnum.LCL || loadtype == LoadTypeEnum.LTL || loadtype == LoadTypeEnum.PENDING)
                        {
                            // LCL loads can only have one schedule !!
                            fm.Schedules = fm.Items.Select(s => new FreightMovementResourceScheduleInfo
                            {
                                GoodsReady = s.GoodsReady.GetValueOrDefault(),
                                PlaceOfLoading = $"({s.PlaceOfLoadingInfo.Country.Code}) - {s.PlaceOfLoadingInfo.Name}",
                                ScheduleName = s.Description,
                                PlaceOfLoadingId = new Guid(s.PlaceOfLoadingId.ToString())
                            }).Take(1).ToList();
                        }
                        else
                        {
                            fm.Schedules = fm.Items.Select(s => new FreightMovementResourceScheduleInfo
                            {
                                GoodsReady = s.GoodsReady.GetValueOrDefault(),
                                PlaceOfLoading = $"({s.PlaceOfLoadingInfo.Country.Code}) - {s.PlaceOfLoadingInfo.Name}",
                                ScheduleName = s.Description,
                                PlaceOfLoadingId = new Guid(s.PlaceOfLoadingId.ToString())
                            }).ToList();
                        }

                        return fm;
                    }
                }

                var movement = await _context.FreightMovements
                    .Include(i => i.PlaceOfLoading)
                    .Include(i => i.PortOfLoading)
                    .Include(i => i.PlaceOfDispatch)
                    .Include(i => i.PortOfDischarge)
                    .Include(i => i.Items).ThenInclude(i => i.CargoItems).ThenInclude(i => i.Product)
                    .SingleOrDefaultAsync(q => q.CompanyId == request.OrganisationId && q.Id == request.FreightMovementId && q.IsActive);

                //var movement = await _context.FreightMovements
                //    .Include(i => i.PortOfLoading)
                //    .Include(i => i.PortOfDischarge)
                //    .Include(i => i.PlaceOfLoading)
                //    .Include(i => i.PlaceOfDispatch)
                //    .Include(i => i.Items).ThenInclude(i => i.CargoItems).ThenInclude(t => t.Product)
                //    .Include(i => i.Items).ThenInclude(i => i.CargoItems).ThenInclude(t => t.ProductVariant)
                //    .Include(i => i.Items).ThenInclude(i => i.CargoItems).ThenInclude(t => t.PurchaseOrderItemScheduleLine).ThenInclude(t => t.PlaceOfLoading)
                //    .AsNoTracking()
                //    .SingleOrDefaultAsync(q => q.Id == quotation_request.FreightMovementId);

                if (movement == null) throw new NotFoundException("FreightMovement", request.FreightMovementId);

                fm = movement.Adapt<FreightMovementResource>();
                List<FCLItem> fcl_items = new List<FCLItem>();
                List<LCLItem> lcl_items = new List<LCLItem>();

                foreach (var item in movement.Items)
                {
                    if (item.ContainerTypeCode != null)
                    {
                        FCLItem fcl = new FCLItem
                        {
                            HazardCode = item.HazardCode,
                            ContainerType = item.ContainerTypeCode,
                            CargoItems = new List<CargoItem>()
                        };
                        foreach (var ci in item.CargoItems)
                        {
                            fcl.CargoItems.Add(new CargoItem
                            {
                                ProductId = ci.ProductId,
                                HsCode = ci.HsCode,
                                SKU = ci.SKU,
                                Qty = ci.Qty,
                                Width = ci.Width,
                                Length = ci.Length,
                                Height = ci.Height,
                                UOL = ci.UOL,
                                UOW = ci.UOW,
                                Weight = ci.Weight,
                                Product = ci.Product
                            });
                        }
                        fcl_items.Add(fcl);
                    }
                    else
                    {
                        LCLItem lcl = new LCLItem
                        {
                            HazardCode = item.HazardCode,
                            CartonQty = item.CartonQty
                        };
                        foreach (var ci in item.CargoItems)
                        {
                            lcl.ProductId = ci.ProductId;
                            lcl.HsCode = ci.HsCode;
                            lcl.SKU = ci.SKU;
                            lcl.Qty = ci.Qty;
                            lcl.Width = ci.Width;
                            lcl.Length = ci.Length;
                            lcl.Height = ci.Height;
                            lcl.UOL = ci.UOL;
                            lcl.UOW = ci.UOW;
                            lcl.Weight = ci.Weight;
                            lcl.Product = ci.Product;
                        }
                        lcl_items.Add(lcl);
                    }
                }
                fm.FCL = fcl_items.Adapt<List<FCLItemResource>>();
                fm.LCL = lcl_items.Adapt<List<LCLItemResource>>();

                return fm;
            }
        }
    }
}

