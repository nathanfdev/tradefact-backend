using Core.Dtos.PurchaseOrder;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemCreateCommand : IRequest<List<Guid>>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public System.Collections.Generic.List<PurchaseOrderItemEditDto> Upsert { get; private set; }

        public PurchaseOrderItemCreateCommand()
        {

        }

        public PurchaseOrderItemCreateCommand(Guid organisationId, Guid purchaseOrderId, List<PurchaseOrderItemEditDto> upsert)
        {
            this.OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.Upsert = upsert;
        }
    }

    public class PurchaseOrderItemCreateCommandHandler : IRequestHandler<PurchaseOrderItemCreateCommand, List<Guid>>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderCreateCommandHandler> _logger;

        public PurchaseOrderItemCreateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderCreateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Guid>> Handle(PurchaseOrderItemCreateCommand cmd, CancellationToken cancellationToken)
        {
            List<Guid> po_lines_created = new List<Guid>();

            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == cmd.PurchaseOrderId && c.CompanyId == cmd.OrganisationId, cancellationToken);

            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", cmd.PurchaseOrderId);

            foreach (PurchaseOrderItemEditDto line in cmd.Upsert)
            {
                PurchaseOrderItem item = new PurchaseOrderItem();
                item.Id = Guid.NewGuid();
                item.ProductId = line.ProductId.GetValueOrDefault();
                item.OrderQuantity = line.OrderQuantity.GetValueOrDefault();
                item.IsBulkUpload = false;

                ProductSupplier prodsupp = await _context.ProductSuppliers.FirstOrDefaultAsync(x => x.SupplierId == existingPurchaseOrder.SupplierId && x.ProductId == line.ProductId && x.IsActive);
                if (prodsupp != null)
                {
                    ProductSupplierCurrency psCurrency = await _context.ProductSupplierCurrency.SingleOrDefaultAsync(x => x.ProductSupplierId == prodsupp.Id && x.CurrencyCode == existingPurchaseOrder.CurrencyId && x.IsActive);

                    if (psCurrency != null) {
                        item.OrderPriceUnit = psCurrency.Price ?? 0;
                    } else
                    {
                        item.OrderPriceUnit = 0;
                    }
                        
                    item.SupplierReference = prodsupp.SupplierReference;
                }
                po_lines_created.Add(item.Id);
                existingPurchaseOrder.PurchaseOrderItems.Add(item);
            }
            _ = await _context.SaveChangesAsync();

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(cmd.PurchaseOrderId));
            return po_lines_created;
        }

    }
}
