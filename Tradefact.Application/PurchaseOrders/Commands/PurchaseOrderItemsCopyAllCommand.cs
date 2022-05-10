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
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemCopyAllCommand  : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid SourcePurchaseOrderId { get; private set; }
        public Guid DestinationPurchaseOrderId { get; private set; }

        public PurchaseOrderItemCopyAllCommand (Guid organisationId, Guid sourcePurchaseOrderId, Guid destinationPurchaseOrderId)
        {
            OrganisationId = organisationId;
            this.SourcePurchaseOrderId = sourcePurchaseOrderId;
            this.DestinationPurchaseOrderId = destinationPurchaseOrderId;
        }
    }

    public class PurchaseOrderItemCopyAllCommandHandler : IRequestHandler<PurchaseOrderItemCopyAllCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderItemCopyAllCommandHandler> _logger;
        private readonly IDbConnection _connection;
        public ICurrentUserService _currentUserService { get; private set; }

        public PurchaseOrderItemCopyAllCommandHandler(IDbConnection connection, IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderItemCopyAllCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connection;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(PurchaseOrderItemCopyAllCommand  command, CancellationToken cancellationToken)
        {
            var existingSourcePurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.SourcePurchaseOrderId, cancellationToken);
            if (existingSourcePurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.SourcePurchaseOrderId);

            var existingDestinationPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.DestinationPurchaseOrderId, cancellationToken);
            if (existingDestinationPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.DestinationPurchaseOrderId);

            using (IDbConnection conn = _connection)
            {
                conn.Open();

                string sql = @$"
                    DECLARE @action_date DATETIME = GETDATE();

                    BEGIN TRY
                        BEGIN TRANSACTION

                             ;WITH cte_insert_values AS(
                                 SELECT  ROW_NUMBER() OVER(ORDER BY CreationDateInternal ASC) AS[Row#], 
                                        @dest_po_id[PurchaseOrderId], NEWID()[Id], Active,
                                         ProductId, SKU, PurchaseOrderItemText, OrderQuantity, OrderQuantityUnit, OrderPriceUnit,
                                         NetPriceAmount, NetPriceQuantity, TaxCode, TaxCountry, TaxJurisdiction, TaxDeterminationDate,
                                         IsDeliveryComplete, IsFinallyInvoiced, PurchaseOrderItemCategory, AccountAssignmentCategory, PurchaseContract,
                                         ItemNetWeight, ItemWeightUnit, ItemVolume, ItemVolumeUnit,
                                         @action_user[CreatedByUser], @action_date[CreationDateInternal], @action_user[LastChangeUser], @action_date[LastModifiedOnInternal]
 
                                     FROM[dbo].[PurchaseOrderItems] PI
 
                                 WHERE PI.PurchaseOrderId = @src_po_id and Active = 1
                             ) 
                             INSERT INTO[dbo].[PurchaseOrderItems]
                             (
                                 PurchaseOrderId, Id, Active,
                                 ProductId, SKU, PurchaseOrderItemText, OrderQuantity, OrderQuantityUnit, OrderPriceUnit,
                                 NetPriceAmount, NetPriceQuantity, TaxCode, TaxCountry, TaxJurisdiction, TaxDeterminationDate,
                                 IsDeliveryComplete, IsFinallyInvoiced, PurchaseOrderItemCategory, AccountAssignmentCategory, PurchaseContract,
                                 ItemNetWeight, ItemWeightUnit, ItemVolume, ItemVolumeUnit,
                                 CreatedByUser, CreationDateInternal, LastChangeUser, LastModifiedOnInternal
                             )
 
                             SELECT  PurchaseOrderId, Id, Active,
                                 ProductId, SKU, PurchaseOrderItemText, OrderQuantity, OrderQuantityUnit, OrderPriceUnit,
                                 NetPriceAmount, NetPriceQuantity, TaxCode, TaxCountry, TaxJurisdiction, TaxDeterminationDate,
                                 IsDeliveryComplete, IsFinallyInvoiced, PurchaseOrderItemCategory, AccountAssignmentCategory, PurchaseContract,
                                 ItemNetWeight, ItemWeightUnit, ItemVolume, ItemVolumeUnit,
                                 CreatedByUser, DATEADD(ms,[Row#]*50, CreationDateInternal), LastChangeUser, DATEADD(ms,[Row#]*50, CreationDateInternal)
                             FROM cte_insert_values
 

                         COMMIT TRAN-- Transaction Success!
                     END TRY
                     BEGIN CATCH
 
                         IF @@TRANCOUNT > 0
 
                             ROLLBACK TRAN          --RollBack in case of Error
 

                             DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
 
                             DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
 
                             DECLARE @ErrorState INT = ERROR_STATE()

                         -- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
 
                         RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                     END CATCH";

                var delete_result = conn.Execute(sql, new {
                    @src_po_id = command.SourcePurchaseOrderId,
                    @dest_po_id = command.DestinationPurchaseOrderId,
                    @action_user = this._currentUserService.Email
                });

                conn.Close();
            }

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.DestinationPurchaseOrderId));
            return command.DestinationPurchaseOrderId;
        }


    }
}
