using Core.Dtos.PurchaseOrder;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderItemUpdateCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid PurchaseOrderItemId { get; private set; }
        public PurchaseOrderItemEditDto Upsert { get; private set; }

        public PurchaseOrderItemUpdateCommand(Guid organisationId, Guid purchaseOrderId, Guid purchaseOrderItemId, PurchaseOrderItemEditDto upsert)
        {
            Upsert = upsert;
            OrganisationId = organisationId;
            this.PurchaseOrderId = purchaseOrderId;
            this.PurchaseOrderItemId = purchaseOrderItemId;
        }
    }

    public class PurchaseOrderItemUpdateCommandHandler : IRequestHandler<PurchaseOrderItemUpdateCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderItemUpdateCommandHandler> _logger;

        public PurchaseOrderItemUpdateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderItemUpdateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderItemUpdateCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            PurchaseOrderItem item = await _context.PurchaseOrderItems.SingleAsync(q => q.Id == command.PurchaseOrderItemId && q.PurchaseOrderId == command.PurchaseOrderId);
            item = command.Upsert.Adapt(item);
            _ = await _context.SaveChangesAsync();

            if (!item.IsBulkUpload)
            {
                var existingProductSupplier = await _context.ProductSuppliers
                .Include(ps => ps.Currencies)
                .SingleOrDefaultAsync(ps => ps.ProductId == item.ProductId && ps.SupplierId == existingPurchaseOrder.SupplierId.Value && ps.IsActive);

                var supplier = await _context.Organisations.SingleOrDefaultAsync(q => q.Id == existingPurchaseOrder.SupplierId);

                if (supplier == null) throw new NotFoundException("Supplier", existingPurchaseOrder.SupplierId);

                var currencyCode = existingPurchaseOrder.CurrencyId ?? supplier.Currency;

                var newCurrency = new ProductSupplierCurrency
                {
                    CurrencyCode = currencyCode,
                    Price = command.Upsert.OrderPriceUnit
                };

                if (existingProductSupplier == null)
                {
                    var newCurrencyList = new List<ProductSupplierCurrency> { newCurrency };

                    ProductSupplier newProductSupplier = new ProductSupplier
                    {
                        ProductId = item.ProductId,
                        SupplierId = existingPurchaseOrder.SupplierId.Value,
                        SupplierReference = command.Upsert.SupplierReference,
                        Price = command.Upsert.OrderPriceUnit,
                        Currencies = newCurrencyList
                    };

                    _context.ProductSuppliers.Add(newProductSupplier);
                } else
                {
                    //If PO currency is not present on the existing PS, add it to the list
                    if (existingProductSupplier.Currencies != null)
                    {
                        var supplierHasCurrency = existingProductSupplier.Currencies.Exists(x => x.CurrencyCode == currencyCode && x.IsActive);
                        if (!supplierHasCurrency)
                        {
                            existingProductSupplier.Currencies.Add(newCurrency);
                        } else
                        {
                            var currentProductSupplierCurrency = existingProductSupplier.Currencies.SingleOrDefault(c => c.CurrencyCode == currencyCode && c.Price != command.Upsert.OrderPriceUnit && c.IsActive);
                            if (currentProductSupplierCurrency != null)
                            {
                                var hasPriceChanged = currentProductSupplierCurrency.Price != command.Upsert.OrderPriceUnit;

                                if (hasPriceChanged)
                                {
                                    currentProductSupplierCurrency.Price = command.Upsert.OrderPriceUnit;
                                }
                            }
                        }
                    } else
                    {
                        existingProductSupplier.Currencies = new List<ProductSupplierCurrency>();
                        existingProductSupplier.Currencies.Add(newCurrency);
                    }

                    existingProductSupplier.SupplierReference = command.Upsert.SupplierReference;
                }
                _ = await _context.SaveChangesAsync();
            }

            // Recalculate Totals
            _ = await _mediator.Send(new PurchaseOrderRecalculateCommand(command.PurchaseOrderId));

            return item.Id;
        }
    }
}
