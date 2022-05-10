using Core.Dtos.PurchaseOrder;
using Core.Models;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderRecalculateCommand : IRequest<Guid>
    {
        public Guid PurchaseOrderId { get; private set; }

        public PurchaseOrderRecalculateCommand(Guid purchaseOrderId)
        {
            this.PurchaseOrderId = purchaseOrderId;
        }
    }

    public class PurchaseOrderRecalculateCommandHandler : IRequestHandler<PurchaseOrderRecalculateCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly IDbConnection _connection;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderRecalculateCommandHandler> _logger;

        public PurchaseOrderRecalculateCommandHandler(IMediator mediator, TradefactDbContext context, IDbConnection connection, ILogger<PurchaseOrderRecalculateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderRecalculateCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);


            string update_sql = @$"
declare @TaxRate decimal(9,2) = 0;
declare @TaxRateM decimal(9,2) = 0

SELECT @TaxRate = ISNULL(SUM(Rate),0) FROM [dbo].[PurchaseOrderAdditionalCharges] AC 
WHERE AC.PurchaseOrderId = @PurchaseOrderId and AC.[Type]=1 and AC.IsActive=1;;

SELECT @TaxRateM = @TaxRate / 100.00

UPDATE P SET    P.TaxRate = @TaxRate, 
                P.BaseCurrency_NetAmount = 0, P.BaseCurrency_TaxAmount = 0, P.BaseCurrency_TotalAmount = 0,
                P.ItemsTotal_NetAmount = 0, ItemsTotal_TaxAmount = 0, ItemsTotal_TotalAmount = 0,
                P.ChargesTotal_NetAmount = 0, ChargesTotal_TaxAmount = 0, ChargesTotal_TotalAmount = 0,
                P.Total_NetAmount = 0, Total_TaxAmount = 0, Total_TotalAmount = 0, P.NumberOfItems = 0
FROM [dbo].[PurchaseOrders] P
WHERE P.Id = @PurchaseOrderId


;WITH cte_line_values AS (
    SELECT POI.PurchaseOrderId, POI.Id [ItemId], 'I' [Type], [OrderQuantity] [LI_OrderQuantity], 
    ISNULL([OrderPriceUnit], 0) [LI_OrderPriceUnit], 
    (ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) [LI_NetAmount], 
    ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM) [LI_Vat],
    ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0)) + ((ISNULL(OrderQuantity,0) * ISNULL([OrderPriceUnit], 0))*@TaxRateM)) [LI_TotalAmount],
    0 [CI_OrderQuantity], 0 [CI_OrderPriceUnit], 0 [CI_NetAmount], 0 [CI_Vat], 0 [CI_TotalAmount], 1 [Lines]
    FROM [dbo].[PurchaseOrderItems] POI 
    WHERE POI.PurchaseOrderId = @PurchaseOrderId and POI.[Active] = 1
    UNION
    SELECT POC.PurchaseOrderId, POC.Id [ItemId], 'C' [Type],
    0 [LI_OrderQuantity], 0 [LI_OrderPriceUnit], 0 [LI_NetAmount], 0 [LI_Vat], 0 [LI_TotalAmount], 
    1 [CI_OrderQuantity], 
    ISNULL(Rate,0) [CI_OrderPriceUnit], 
    1 * ISNULL(Rate,0) [CI_NetAmount], 
    (1 * ISNULL(Rate,0))*(0) [CI_Vat], 
    ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(0))) [CI_TotalAmount], 0 [Lines] 
-- (1 * ISNULL(Rate,0))*(@TaxRateM) [Vat],
-- ((1 * ISNULL(Rate,0)) + ((1 * ISNULL(Rate,0))*(@TaxRateM))) [TotalAmount]
    FROM [dbo].[PurchaseOrderAdditionalCharges] POC 
    WHERE POC.PurchaseOrderId = @PurchaseOrderId and POC.[IsActive] = 1 and POC.[Type] = 2
),
cte_order_totals AS (
    SELECT PurchaseOrderId, 
    SUM(LI_NetAmount) [I_NetAmount], 
    SUM(LI_Vat) [I_Vat], 
    SUM(LI_TotalAmount) [I_TotalAmount], 
    SUM(Lines) [NoOfItems], 

    SUM(CI_NetAmount) [C_NetAmount], 
    SUM(CI_Vat) [C_Vat], 
    SUM(CI_TotalAmount) [C_TotalAmount],

    SUM(LI_NetAmount) + SUM(CI_NetAmount) [NetAmount], 
    SUM(LI_Vat) + SUM(CI_Vat) [Vat], 
    SUM(LI_TotalAmount) + SUM(CI_TotalAmount) [TotalAmount]  

    FROM cte_line_values
    GROUP BY PurchaseOrderId
)
UPDATE P
    SET P.TaxRate = @TaxRate, 
        P.BaseCurrency_NetAmount = T.NetAmount, P.BaseCurrency_TaxAmount = T.Vat, P.BaseCurrency_TotalAmount = T.TotalAmount,
        P.ItemsTotal_NetAmount = T.I_NetAmount, ItemsTotal_TaxAmount = T.I_Vat, ItemsTotal_TotalAmount = T.I_TotalAmount,
        P.ChargesTotal_NetAmount = T.C_NetAmount, ChargesTotal_TaxAmount = T.C_Vat, ChargesTotal_TotalAmount = T.C_TotalAmount,
        P.Total_NetAmount = T.NetAmount, Total_TaxAmount = T.Vat, Total_TotalAmount = T.TotalAmount, P.NumberOfItems = T.NoOfItems
FROM [dbo].[PurchaseOrders] P
    INNER JOIN cte_order_totals T ON T.PurchaseOrderId = P.Id
WHERE P.Id = @PurchaseOrderId
            ";

            using ( IDbConnection conn = _connection)
            {
                conn.Open();
                var result = conn.Execute(update_sql, new
                {
                    @PurchaseOrderId = command.PurchaseOrderId
                });
            }
            return command.PurchaseOrderId;
        }
    }
}
