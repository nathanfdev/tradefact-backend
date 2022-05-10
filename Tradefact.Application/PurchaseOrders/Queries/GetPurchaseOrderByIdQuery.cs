using Core.Enums;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderResource>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid PurchaseOrderId { get; set; }

        public bool includeItems { get; set; } = false;


        public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderResource>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;
            protected readonly IMediator _mediator;

            public GetPurchaseOrderByIdQueryHandler(IDbConnection connection, ICurrentUserService userService, IMediator mediator)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));

                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<PurchaseOrderResource> Handle(GetPurchaseOrderByIdQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$"
;WITH cte_purchaseorders AS (
    SELECT P.[Id] [PurchaseOrderId], P.CompanyId, P.SupplierId
    FROM [dbo].[PurchaseOrders] P
    WHERE P.[Id] = @PurchaseOrderId  
),
cte_freightmovement_info AS (
    SELECT P.[PurchaseOrderId], 
        MAX(CASE WHEN FM.Id IS NOT NULL THEN 1 ELSE 0 END) [ShippingQuote_Requested],
        SUM(CASE WHEN QR.[State] = 0 THEN 1 ELSE 0 END) [ShippingQuote_Pending],
        SUM(CASE WHEN QR.[State] = 1 THEN 1 ELSE 0 END) [ShippingQuote_Ready],
        SUM(CASE WHEN QR.[State] = 2 THEN 1 ELSE 0 END) [ShippingQuote_Accepted],
        SUM(CASE WHEN QR.[State] = 3 THEN 1 ELSE 0 END) [ShippingQuote_Expired],
        SUM(CASE WHEN QR.[State] = 4 THEN 1 ELSE 0 END) [ShippingQuote_Rejected]
    FROM cte_purchaseorders P
        LEFT JOIN [dbo].[FreightMovements] FM ON FM.Id = P.PurchaseOrderId
        LEFT JOIN [dbo].[QuotationRequests] QR ON QR.FreightMovementId = FM.Id
    GROUP BY P.[PurchaseOrderId]
)
, cte_product_hscodes AS (
    SELECT DISTINCT PRD.HsCode
    FROM [dbo].[PurchaseOrders] P
        LEFT JOIN [dbo].PurchaseOrderItems PI ON PI.PurchaseOrderId = P.Id
        LEFT JOIN [dbo].[Products] PRD ON PRD.Id = PI.ProductId
    WHERE P.[Id] = @PurchaseOrderId  and PI.Active = 1 and PI.ProductId IS NOT NULL
)
, cte_hscodes AS (
    SELECT @PurchaseOrderId [PurchaseOrderId],  STRING_AGG(HsCode, ',') WITHIN GROUP (ORDER BY HsCode) AS HSCodes FROM cte_product_hscodes
)
,cte_raw as (
    SELECT  PO.Active, P.[PurchaseOrderId], PO.PurchaseOrderNumber, PO.Reference, PO.CompanyId, SU.Name [Supplier], CL.Name [Customer], PO.SupplierId, 
        PL.LocCode [PortofLoadingId], '(' + PL.CountryCode + ') - ' + PL.Name [PortofLoading],
        PD.LocCode [PortofDischargeId], '(' + PD.CountryCode + ') - ' + PD.Name [PortofDischarge], 
        PO.PlaceOfLoadingId, 
        '(' + AL.CountryCode + ') - ' + AL.Name [PlaceofLoading],
        AL.Name [PlaceofLoading_Name],
        AL.AddressLine1 [PlaceofLoading_AddressLine1],
        AL.AddressLine2 [PlaceofLoading_AddressLine2],
        AL.AddressLine3 [PlaceofLoading_AddressLine3],
        AL.AddressLine4 [PlaceofLoading_AddressLine4],
        AL.City [PlaceofLoading_City],
        AL.PostalCode [PlaceofLoading_PostalCode],
        AL.CountryCode [PlaceofLoading_Country_Code],
        ALC.Name [PlaceofLoading_Country_Name],
        PO.PlaceOfDispatchId, 
        '(' + AD.CountryCode + ') - ' + AD.Name [PlaceofDispatch],
        AD.Name [PlaceofDispatch_Name],
        AD.AddressLine1 [PlaceofDispatch_AddressLine1],
        AD.AddressLine2 [PlaceofDispatch_AddressLine2],
        AD.AddressLine3 [PlaceofDispatch_AddressLine3],
        AD.AddressLine4 [PlaceofDispatch_AddressLine4],
        AD.City [PlaceofDispatch_City],
        AD.PostalCode [PlaceofDispatch_PostalCode],
        AD.CountryCode [PlaceofDispatch_Country_Code],
        ADC.Name [PlaceofDispatch_Country_Name],
        PO.GoodsReadyDate,
        PO.PurchaseOrderDate,
        PO.TargetDeliveryDate,
        PO.CurrencyId,
        PO.DateOfIssue,
        PO.RejectedReason,
        PO.PaymentTerms,
        PO.OrderRejected,
        PO.Tags, PO.Containers, PO.Status,
        PO.Submitted, PO.SubmittedDate, PO.Accepted, PO.AcceptedDate, PO.Rejected, PO.RejectedDate, 
        PO.InProduction, PO.InProductionDate, PO.Shipping, PO.ShippedDate, PO.Completed, PO.CompletedDate, 
        PO.PreShipment, PO.PreShipmentDate, PO.Cancelled, PO.CancelledDate,
        PO.TransactionType, PO.LoadType, PO.IncoTerms, PO.ShipmentType, SU.Name [Provider], ISNULL(PO.CurrencyId,  SU.Currency) [Currency], CL.Name [Client], PO.CreationDateInternal, PO.CreatedByUser, AU.FullName [CreatedByName], PO.Notes, PO.Signature, PO.AdditionalInformation,
        ISNULL(PO.TaxRate,0) [TaxRate], 
        ISNULL(PO.BaseCurrency_NetAmount,0) [BaseCurrency_NetAmount], ISNULL(PO.BaseCurrency_TaxAmount,0) [BaseCurrency_TaxAmount], ISNULL(PO.BaseCurrency_TotalAmount,0) [BaseCurrency_TotalAmount], 

        ISNULL(PO.ItemsTotal_NetAmount,0) [Items_NetAmount], ISNULL(PO.ItemsTotal_TaxAmount,0) [Items_TaxAmount], ISNULL(PO.ItemsTotal_TotalAmount,0) [Items_TotalAmount],
        ISNULL(PO.ChargesTotal_NetAmount,0) [Charges_NetAmount], ISNULL(PO.ChargesTotal_TaxAmount,0) [Charges_TaxAmount], ISNULL(PO.ChargesTotal_TotalAmount,0) [Charges_TotalAmount],
        ISNULL(PO.Total_NetAmount,0) [Total_NetAmount], ISNULL(PO.Total_TaxAmount,0) [Total_TaxAmount], ISNULL(PO.Total_TotalAmount,0) [Total_TotalAmount],

        PO.InsuranceCurrency, PO.InsuranceRequired, PO.InsuranceValue, PO.CustomsBrokerageRequired, PO.HSCodes, PHC.HSCodes [Product_HSCodes],
        PO.LogisticsNotes, PO.NumberOfItems, PO.IsLocked, SU.ParentId
    FROM cte_purchaseorders P
        INNER JOIN [dbo].[PurchaseOrders] PO ON PO.Id = P.PurchaseOrderId
        LEFT JOIN cte_hscodes PHC ON PHC.PurchaseOrderId = P.PurchaseOrderId
        LEFT JOIN [dbo].[Organisations] SU ON SU.Id = PO.SupplierId
        LEFT JOIN [dbo].[Organisations] CL ON CL.Id = PO.CompanyId
        LEFT JOIN [dbo].[Addresses] AL ON AL.Id = PO.PlaceOfLoadingId
        LEFT JOIN [dbo].[Countries] ALC ON ALC.Code2 = AL.CountryCode
        LEFT JOIN [dbo].[Addresses] AD ON AD.Id = PO.PlaceOfDispatchId
        LEFT JOIN [dbo].[Countries] ADC ON ADC.Code2 = AD.CountryCode
        LEFT JOIN [dbo].[Locations] PL ON PL.LocCode = PO.PortOfLoadingId
        LEFT JOIN [dbo].[Locations] PD ON PD.LocCode = PO.PortOfDischargeId
        LEFT JOIN [dbo].[AspNetUsers] AU ON AU.Email = PO.CreatedByUser

)
SELECT *, 1 [OrganisationType], FM.ShippingQuote_Requested, FM.ShippingQuote_Pending, FM.ShippingQuote_Accepted, FM.ShippingQuote_Ready, FM.ShippingQuote_Rejected, FM.ShippingQuote_Expired, PO.ParentId [SupplierParentId]
FROM cte_raw PO
    LEFT JOIN cte_freightmovement_info FM ON FM.PurchaseOrderId = PO.PurchaseOrderId";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    PurchaseOrderDTO po = await conn.QueryFirstAsync<PurchaseOrderDTO>(sQuery, new
                    {
                        request.OrganisationId,
                        @PurchaseOrderId = request.PurchaseOrderId,
                        @OrganisationType = request.OrganisationType
                    });
                    po.IsOwned = (po.CompanyId == request.OrganisationId);

                    PurchaseOrderResource retValue = po.Adapt<PurchaseOrderResource>();
                    if (request.includeItems)
                    {
                        retValue.PurchaseOrderItems = await _mediator.Send(new GetPurchaseOrderItemsQuery
                        {
                            OrganisationId = request.OrganisationId,
                            PurchaseOrderId = request.PurchaseOrderId
                        });

                        retValue.PurchaseOrderAdditionalChargeItems = await _mediator.Send(new GetPurchaseOrderChargeItemsQuery
                        {
                            OrganisationId = request.OrganisationId,
                            PurchaseOrderId = request.PurchaseOrderId
                        });

                    }
                    return retValue;
                }

            }
        }
    }
}
