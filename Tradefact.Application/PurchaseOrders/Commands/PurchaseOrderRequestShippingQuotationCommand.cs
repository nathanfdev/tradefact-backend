using Core.Dtos.PurchaseOrder;
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
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderRequestShippingQuotationCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        // public PurchaseOrderShipmentQuotationRequest Request { get; set; }

        public PurchaseOrderRequestShippingQuotationCommand(Guid organisationId, Guid purchaseOrderId)
        {
            this.PurchaseOrderId = purchaseOrderId;
            this.OrganisationId = organisationId;
        }
    }

    public class PurchaseOrderRequestShippingQuotationCommandHandler : IRequestHandler<PurchaseOrderRequestShippingQuotationCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly IDbConnection _connection;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderRequestShippingQuotationCommandHandler> _logger;

        public PurchaseOrderRequestShippingQuotationCommandHandler(IMediator mediator, TradefactDbContext context, IDbConnection connection, ILogger<PurchaseOrderRequestShippingQuotationCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderRequestShippingQuotationCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId, cancellationToken);
            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            //existingPurchaseOrder.InsuranceRequired = command.Request.InsuranceRequired;
            //existingPurchaseOrder.InsuranceCurrency = command.Request.InsuranceCurrency;
            //existingPurchaseOrder.InsuranceValue = command.Request.InsuranceValue;

            //existingPurchaseOrder.CustomsBrokerageRequired = command.Request.CustomsBrokerageRequired;
            //if (command.Request.TargetDeliveryDate.HasValue)
            //{
            //    existingPurchaseOrder.TargetDeliveryDate = command.Request.TargetDeliveryDate.GetValueOrDefault();            
            //}

            Partnership partnership = await _context.Partnerships.Include(i => i.Provider).FirstOrDefaultAsync(q => q.ClientId == command.OrganisationId);

            FreightMovement f = await this.BuildFreightMovement(existingPurchaseOrder, command.OrganisationId, cancellationToken);

            f.QuotationRequests.Add(new QuotationRequest
            {
                PartnershipId = partnership.Id,
                Submitted = DateTime.Now,
                State = Core.Enums.QuotationStateEnum.PENDING
            });
            _context.FreightMovements.Add(f);
            _ = await _context.SaveChangesAsync();

            return command.PurchaseOrderId;
        }

        private async Task<FreightMovement> BuildFreightMovement(PurchaseOrder p, Guid organisationId, CancellationToken cancellationToken)
        {

            const string container_type = "00G0";

            FreightMovement f = new FreightMovement();
            //  Freight movement will always share an Id with orginal PO
            f.Id = p.Id;
            // Organisations
            f.CompanyId = organisationId;
            f.SupplierId = p.SupplierId;
            f.Name = p.PurchaseOrderNumber;

            // Assign PO reference to Freight movement
            f.Reference = p.Reference ?? this.GenerateReference();

            f.GoodsReady = p.GoodsReadyDate.GetValueOrDefault();
            if (p.TargetDeliveryDate.HasValue)
            {
                f.DeliveryDate = p.TargetDeliveryDate.GetValueOrDefault();
            }

            f.TransactionType = p.TransactionType;
            f.ShipmentType = p.ShipmentType;
            f.IncoTerms = p.IncoTerms;
            f.LoadType = p.LoadType;

            f.PlaceOfLoadingId = p.PlaceOfLoadingId;
            f.PortOfLoadingId = p.PortOfLoadingId;

            f.PlaceOfDispatchId = p.PlaceOfDispatchId;
            f.PortOfDischargeId = p.PortOfDischargeId;

            f.InsuranceRequired = p.InsuranceRequired;
            f.InsuranceCurrency = p.InsuranceCurrency;
            f.InsuranceValue = p.InsuranceValue;
            f.CustomsBrokerageRequired = p.CustomsBrokerageRequired;
            f.Tags = p.Tags;

            f.Notes = p.LogisticsNotes;
            f.PurchaseOrderId = p.Id;            

            FreightMovementItem container = new FreightMovementItem
            {
                Id = Guid.NewGuid(),
                ContainerTypeCode = container_type,
                CargoItems = new List<CargoItem>()
            };

            List<string> hsCodes = new List<string>();
            if (!String.IsNullOrEmpty(p.HSCodes))
            {
                hsCodes.AddRange(p.HSCodes.Split(',').Distinct());
            }
            var purchaseOrderItems = await _mediator.Send(new GetPurchaseOrderItemsQuery
            {
                OrganisationId = organisationId,
                PurchaseOrderId = p.Id
            });

         

            f.Items = new List<FreightMovementItem>();
            decimal consignmentqty = 0;
            foreach (var item in purchaseOrderItems)
            {
                if (hsCodes.Contains(item.Product.HsCode))
                {
                    hsCodes.Add(item.Product.HsCode);
                }
                try
                {
                    CargoItem c = new CargoItem
                    {
                        ItemId = Guid.NewGuid(),
                        ProductId = Guid.Parse(item.Product.Id),
                        IsProductVariant = item.IsProductVariant,
                        HsCode = item.Product.HsCode,
                        SKU = item.Product.SKU,
                        ItemDescription = item.Product.Name,
                        ProductDescriptionOverride = item.ProductDescriptionOverride,
                        CartonQty = item.OrderQuantity > 0 ? (int)item.OrderQuantity : (int)item.OrderQuantity,
                        Qty = item.OrderQuantity > 0 ? item.OrderQuantity : item.OrderQuantity,
                        Width = item.Product.Dimensions.Width,
                        Length = item.Product.Dimensions.Length,
                        Height = item.Product.Dimensions.Height,
                        Weight = item.Product.Dimensions.Weight
                    };

                    if (c.IsProductVariant)
                    {
                        c.ProductVariantId = c.ProductId;
                        Guid pid = Guid.Parse(item.Product.Id);
                        c.ProductId = await this.GetProductVariantMasterId(pid);
                    }

                    consignmentqty += c.Qty;
                    container.CargoItems.Add(c);

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            f.Items.Add(container);
            f.HSCodes = string.Join(",", hsCodes);
            f.ConsignmentQuantity = consignmentqty;

            return f;
        }

        private async Task<Guid> GetProductVariantMasterId(Guid variantId)
        {
            ProductVariant variant = await _context.ProductVariants.SingleOrDefaultAsync(s => s.Id == variantId);
            return variant.ProductId;
        }

        private string GenerateReference()
        {
            DateTime _now = DateTime.Now;

            StringBuilder builder = new StringBuilder();
            builder.Append(_now.ToString("yy"));
            builder.Append("-");
            builder.Append(_now.ToString("MM"));
            builder.Append("-");

            Random random = new Random();
            char ch;
            for (int i = 0; i < 5; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
