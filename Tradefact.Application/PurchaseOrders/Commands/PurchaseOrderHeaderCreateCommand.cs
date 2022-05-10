using Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class PurchaseOrderCreateCommand : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid? SupplierId { get; private set; }
        public string PONumber { get; private set; }
        public string CurrencyId { get; private set; }

        public PurchaseOrderCreateCommand()
        {

        }

        public PurchaseOrderCreateCommand (Guid organisationId, Guid? supplierId, string currencyId, string? poNumber = default)
        {
            SupplierId = supplierId;
            OrganisationId = organisationId;
            CurrencyId = currencyId;
            this.PONumber = poNumber;
        }
    }

    public class PurchaseOrderCreateCommandHandler : IRequestHandler<PurchaseOrderCreateCommand , Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<PurchaseOrderCreateCommandHandler> _logger;

        public PurchaseOrderCreateCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<PurchaseOrderCreateCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PurchaseOrderCreateCommand  message, CancellationToken cancellationToken)
        {
            PurchaseOrder po = new PurchaseOrder { CompanyId = message.OrganisationId, CurrencyId = message.CurrencyId };
            po.Id = Guid.NewGuid();
            po.Status = PurchaseOrderStatus.Draft;
            po.PurchaseOrderDate = DateTime.Now;

            if (message.SupplierId.HasValue)
            {
                po.SupplierId = message.SupplierId.GetValueOrDefault();
            }
            po.PurchaseOrderNumber = String.IsNullOrEmpty(message.PONumber) ? this.GeneratePONumber() : message.PONumber;
            _context.PurchaseOrders.Add(po);
            // Each purchase order is automatically assigned a comments room
            Room r = new Room
            {
                Id = po.Id,
                IsOpen = true,
                UnreadCount = 0,
            };
            _context.Rooms.Add(r);
            _ = await _context.SaveChangesAsync();

            // Recalculate Totals
            _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(po.Id, PurchaseOrderEventType.Created, DateTime.Now));

            return po.Id;
        }

        private string GeneratePONumber()
        {
            DateTime _now = DateTime.Now;

            StringBuilder builder = new StringBuilder();
            builder.Append("Order");
            builder.Append("-");
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
