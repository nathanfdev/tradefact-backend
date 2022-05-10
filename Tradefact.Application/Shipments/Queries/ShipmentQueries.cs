using Core.Enums;
using Core.Models.Criteria;
using System;
using System.Text;



namespace Tradefact.Application.Shipments.Queries.GetShipmentList
{
    public class ShipmentQueries
    {
        public string GetBuildQuery(ShipmentSearchCriteria criteria, OrganisationTypeEnum organisationType, string email)
        {
            string organisation_type_switcher = organisationType == OrganisationTypeEnum.SHIPPER ? "P.ClientId" : "P.ProviderId";

            StringBuilder where_clause = new StringBuilder();
            if (criteria.CreatedByMe) where_clause.Append($" AND S.CreatedByUser = '{email}'");

            if (criteria.SupplierId.HasValue)
            {
                where_clause.Append($" AND FM.SupplierId = '{criteria.SupplierId}'");
            }
            else if (criteria.BuyerId.HasValue)
            {
                where_clause.Append($" AND FM.BuyerId = '{criteria.BuyerId}'");
            }
            else if (criteria.OrganisationId.HasValue)
            {
                where_clause.Append($" AND (FM.SupplierId = '{criteria.OrganisationId}' OR FM.BuyerId = '{criteria.OrganisationId}')");
            }
            if (criteria.CreatedByMe) where_clause.Append($" AND S.CreatedByUser = '{email}'");

            if (criteria.ShipmentMethod != null) where_clause.Append($" AND S.ShipmentType = @ShipmentMethod");
            if (criteria.Completed)
            {
                where_clause.Append($" AND S.Delivered = 1");
            }
            else
            {
                where_clause.Append($" AND S.Delivered = 0");
                if (criteria.ShipmentStatus != null)
                {
                    where_clause.Append(" AND (S.Active=1 AND ");
                    switch (criteria.ShipmentStatus)
                    {
                        case Core.Models.Criteria.ShipmentStatus.AwaitingCollection:
                            where_clause.Append("S.Collected = 0");
                            break;

                        case Core.Models.Criteria.ShipmentStatus.InTransitToPort:
                            where_clause.Append("S.Collected = 1 AND S.DepartedPOL = 0");
                            break;

                        case Core.Models.Criteria.ShipmentStatus.Shipping:
                            where_clause.Append("S.Collected = 1 AND S.DepartedPOL = 1 AND S.ArrivedPOD = 0");
                            break;

                        case Core.Models.Criteria.ShipmentStatus.PendingCustomsClearance:
                            where_clause.Append("S.ArrivedPOD = 1 and S.CustomsClearence = 0");
                            break;

                        case Core.Models.Criteria.ShipmentStatus.InTransitToDestination:
                            where_clause.Append("S.CustomsClearence = 1 and S.Delivered = 0");
                            break;

                        default:
                            break;
                    }
                    where_clause.Append(")");
                }
            }
            if (criteria.ActiveOnly.GetValueOrDefault())
            {
                // where_clause.Append($" AND (ex_shipment.STATE = { (int)QuotationStateEnum.READY } OR ex_shipment.STATE = {(int)QuotationStateEnum.PENDING})");
            }
            else
            {
                // if (request.Criteria.Status != null) where_clause.Append($" AND ex_shipment.STATE = @status");
            }

            StringBuilder query_builder = new StringBuilder();

            query_builder.AppendLine(@$"
                    DECLARE @shipments TABLE (
                        ShipmentId UNIQUEIDENTIFIER NOT NULL,
                        FreightMovementId UNIQUEIDENTIFIER NOT NULL,
                        PartnershipId UNIQUEIDENTIFIER NOT NULL,
                        LastModifiedOnInternal DATETIME2 NOT NULL,
                        NoOfDocuments  INT DEFAULT 0
                    )

                    ;WITH cte_shipments AS (
                            SELECT S.[Id] [ShipmentId], S.FreightMovementId, S.PartnershipId, S.LastModifiedOnInternal
                            FROM [dbo].[Shipments] S
                            LEFT JOIN [dbo].[FreightMovements] FM ON FM.Id = S.FreightMovementId
                            INNER JOIN [dbo].[Partnerships] P ON P.Id = S.PartnershipId
                            WHERE {organisation_type_switcher} = @OrganisationId{where_clause.ToString()} AND (S.Delivered = 0 OR S.LastModifiedOnInternal > DATEADD(day, -10, GETDATE()))
                    )");

            if (String.IsNullOrEmpty(criteria.Search))
            {
                query_builder.AppendLine(@"
                        INSERT INTO @shipments (ShipmentId, FreightMovementId, PartnershipId, LastModifiedOnInternal)
                        SELECT ShipmentId, FreightMovementId, PartnershipId, LastModifiedOnInternal FROM cte_shipments");
            }
            else
            {
                query_builder.AppendLine(@"
                        ,cte_shipment_sku_filter AS (
                            SELECT S.ShipmentId, S.[FreightMovementId], S.PartnershipId, ISNULL(P.Name,'') + ' ' + ISNULL(P.SKU, '') + ' ' + ISNULL(P.Nickname, '') [Product]
                            FROM cte_shipments S
                            LEFT JOIN [dbo].[CargoItems] CI ON CI.FreightMovementId = S.FreightMovementId
                            LEFT JOIN [dbo].[Products] P ON P.Id = CI.ProductId
                            GROUP BY S.ShipmentId, S.[FreightMovementId], S.PartnershipId, ISNULL(P.Name,'') + ' ' + ISNULL(P.SKU, '') + ' ' + ISNULL(P.Nickname, '')
                        )
                        ,cte_shipment_cargo_items AS (
                            SELECT CR.ShipmentId, CR.[FreightMovementId], CR.PartnershipId, STRING_AGG(CR.Product, ' ') AS Products
                            FROM cte_shipment_sku_filter CR
                            GROUP BY CR.ShipmentId, CR.[FreightMovementId], CR.PartnershipId
                        )
                        ,cte_raw as (
                            SELECT SR.ShipmentId,
                                '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], PL.LocCode [PortofLoadingCode],
                                '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], PD.LocCode [PortofDischargeCode], 
                                FM.GoodsReady, FM.Tags, S.[Tags] [sTags],
                                FM.TransactionType, FM.IncoTerms, FM.ShipmentType, FM.Name, FM.LoadType, FM.Reference, PR.Name [Partner], CL.Name [Client], SR.Products, SU.[Name] [Supplier]
                            FROM cte_shipment_cargo_items SR
                                INNER JOIN [dbo].[Partnerships] P ON P.Id = SR.PartnershipId
                                LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                INNER JOIN [dbo].[Shipments] S ON S.Id = SR.ShipmentId
                                INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = SR.FreightMovementId
                                LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = S.PortOfLoadingId
                                LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = S.PortOfDischargeId
                        ),
                        cte_orders AS (
                            SELECT ShipmentId, STRING_AGG(PurchaseOrderNumber + ' ' + Reference, ' ') [PurchaseOrderSearch]
                            FROM (
                                SELECT DISTINCT CR.ShipmentId, PO.PurchaseOrderNumber, PO.Reference
                                FROM cte_shipment_sku_filter CR
                                JOIN FreightMovementItems FMI ON FMI.FreightMovementId = CR.FreightMovementId
                                JOIN PurchaseOrders PO ON PO.Id = FMI.PurchaseOrderId
                            ) a
                            GROUP BY ShipmentId
                        ),
                        cte_search as (
                            SELECT CS.*, ISNULL(CS.Client, '') + ' ' + ISNULL(CS.Supplier, '') + ' ' + 
                            REPLACE(ISNULL(CS.Tags, ''), ',', ' ') + ' ' + ISNULL(CS.PlaceofLoading, '') + ' ' + ISNULL(CS.PortofLoading, '') + ' ' +
                            ISNULL(CS.PlaceofDischarge, '') + ' ' + ISNULL(CS.PortofDischarge, '') + ' ' +
                            ISNULL(CS.Reference, '') + ' ' + ISNULL(CS.Name, '') + ' ' + ISNULL(CS.Products, '') + ' ' + ISNULL(CS.sTags, '') + ' ' +
                            ISNULL(CO.PurchaseOrderSearch, '') [Search]
                            FROM cte_raw CS
                            LEFT OUTER JOIN cte_orders CO ON CO.ShipmentId = CS.ShipmentId
                        )        
                        INSERT INTO @shipments (ShipmentId, FreightMovementId, PartnershipId, LastModifiedOnInternal)
                        SELECT CS.ShipmentId, S.FreightMovementId, S.PartnershipId, S.LastModifiedOnInternal 
                        FROM cte_search CS
                            INNER JOIN cte_shipments S ON S.ShipmentId = CS.ShipmentId
                        WHERE CS.Search like @search");
            }

            query_builder.AppendLine(@"
                    UPDATE S SET S.NoOfDocuments = NoDocuments FROM (
                        SELECT S.ShipmentId, COUNT(SD.DocumentId) [NoDocuments]
                        FROM @shipments S
                            INNER JOIN [ShipmentDocuments] SD ON SD.ShipmentId = S.ShipmentId
                            INNER JOIN [Documents] D ON D.Id = SD.DocumentId
                        WHERE SD.IsActive = 1 AND D.Active = 1
                        GROUP BY S.ShipmentId
                    ) DT
                        INNER JOIN @shipments S ON S.ShipmentId = DT.ShipmentId
                ");


            query_builder.AppendLine(@"
                                    SELECT  CS.ShipmentId [Id],
                                            '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
                                            '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading], 
                                            PL.LocCode [PortofLoadingCode],
                                            '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDischarge], 
                                            '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
                                            PD.LocCode [PortofDischargeCode],
                                            FM.GoodsReady, FM.IncoTerms, FM.ShipmentType, FM.Name, ISNULL(S.LoadType, FM.LoadType) [LoadType], FM.Reference, FM.TransactionType, 
                                            PR.Name [Partner], CL.Name [Client], '' [Products], SU.Name [Supplier], 
                                            S.Tags, S.CollectionDate, S.Status, S.Stage,
                                            S.[ArrivedPOD], S.[ArrivedPODDate], S.[Booked], S.[BookedDate], S.[Collected], S.[CustomsClearence], S.[CustomsClearenceDate], 
                                            S.[Delivered], S.[DeliveryDate], S.[EquipmentTrackAvailable], S.[EstimatedCollectionDate], S.[InCustoms], S.[InTransit], 
                                            S.[InTransitDate], S.[IssueAtCustomCleared], S.[IssueAtCustoms], S.[IssueAtCustomsDate], S.[ShipmentTrackAvailable],
                                            S.[SCAC], S.[BillofLadingNumber], S.[IMO], S.[VesselName], S.[Latitude], S.[Longitude],
                                            S.[DepartedPOL],S.[DepartedPOLDate],S.[EstimatedDeliveryDate],S.[TrackinformationAdded], S.[ETA] [EstimatedArrivalPOD], S.[ETD] [EstimatedDeparturePOL], CS.ContainerList, CS.NoOfDocuments
                                            FROM (  SELECT S.ShipmentId, FreightMovementId, PartnershipId, S.LastModifiedOnInternal,  STRING_AGG(E.ContainerNo, ',') AS ContainerList, S.NoOfDocuments
                                                    FROM  @shipments S
                                                        LEFT JOIN [dbo].[EquipmentAllocations] E ON E.ShipmentId = S.ShipmentId
                                                    GROUP BY S.ShipmentId, S.[FreightMovementId], S.PartnershipId, S.LastModifiedOnInternal, S.NoOfDocuments
                                                    ORDER BY S.LastModifiedOnInternal desc
                                                    OFFSET (@PageNumber-1)*@PageSize ROWS
                                                    FETCH NEXT @PageSize ROWS ONLY
                                            ) CS
                                        INNER JOIN [dbo].[Shipments] S ON S.Id = CS.ShipmentId
                                        INNER JOIN [dbo].[Partnerships] P ON P.Id = CS.PartnershipId
                                        INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = CS.FreightMovementId
                                        LEFT JOIN [dbo].[Organisations] PR ON PR.Id = P.ProviderId
                                        LEFT JOIN [dbo].[Organisations] CL ON CL.Id = P.ClientId
                                        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = FM.PlaceOfLoadingId
                                        LEFT JOIN [dbo].[Organisations] SU ON SU.Id = AL.OrganisationId
                                        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = FM.PlaceOfDispatchId
                                        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = S.PortOfLoadingId
                                        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = S.PortOfDischargeId

                                        SELECT  QRS.*, 
                                                PO.PurchaseOrderNumber [PurchaseOrderNumber], PO.Reference [PurchaseOrderReference], 
                                                O.Id [SupplierId], O.Name [SupplierName],
                                                '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading] FROM (
                                            SELECT CS.[ShipmentId], FM.Id [FreightMovementId], 
                                                    PSL.[PurchaseOrderId] [PurchaseOrderId],
                                                    ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId) [PlaceOfLoadingId], ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady) [GoodsReady], PSL.[Description] [ScheduleName]
                                            FROM (  
                                                        SELECT S.ShipmentId, FreightMovementId, PartnershipId, S.LastModifiedOnInternal
                                                        FROM  @shipments S
                                                        ORDER BY S.LastModifiedOnInternal desc
                                                        OFFSET (@PageNumber-1)*@PageSize ROWS
                                                        FETCH NEXT @PageSize ROWS ONLY
                                                    ) CS
                                                INNER JOIN [dbo].[FreightMovements] FM ON FM.Id = CS.FreightMovementId
                                                INNER JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = FM.Id
                                                LEFT JOIN [dbo].[PurchaseOrderItemScheduleLines] PSL ON PSL.PurchaseOrderId = FMI.PurchaseOrderId AND PSL.Id = FMI.PurchaseOrderItemScheduleLineId
                                            GROUP BY CS.ShipmentId, FM.Id, PSL.[PurchaseOrderId], ISNULL(PSL.PlaceOfLoadingId, FM.PlaceOfLoadingId), ISNULL(PSL.ConfirmedGoodsReadyDate, FM.GoodsReady), PSL.[Description] 
                                        ) QRS
                                            LEFT JOIN [dbo].[PurchaseOrders] PO ON PO.Id = QRS.PurchaseOrderId
                                            LEFT JOIN [dbo].[Organisations] O ON O.Id = PO.SupplierId
                                            LEFT JOIN [dbo].[Addresses] AL ON AL.Id = QRS.PlaceOfLoadingId
                                        ORDER BY QRS.ShipmentId, PlaceOfLoadingId

                                    SELECT DISTINCT S.ShipmentId, PO.Id [PurchaseOrderId], PO.PurchaseOrderNumber
                                    FROM @shipments S
                                    JOIN [dbo].[FreightMovementItems] FMI ON FMI.FreightMovementId = s.FreightMovementId
                                    JOIN [dbo].[PurchaseOrders] PO ON PO.Id = FMI.PurchaseOrderId

                                    SELECT COUNT(ShipmentId) [Quotes] FROM @shipments");

            return query_builder.ToString();
        }
    }
}
