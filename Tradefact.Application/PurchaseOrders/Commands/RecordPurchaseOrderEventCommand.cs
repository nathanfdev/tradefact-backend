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
    public class RecordPurchaseOrderEventCommand : IRequest<bool>
    {
        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrderEventType EvenType { get; private set; }
        public DateTime EventTime { get; private set; }
        public string Title { get; private set; }

        public RecordPurchaseOrderEventCommand(Guid purchaseOrderId, PurchaseOrderEventType eventype, DateTime? eventtime = null, string title = null)
        {
            this.PurchaseOrderId = purchaseOrderId;
            this.EvenType = eventype;
            this.EventTime = eventtime.HasValue ? eventtime.GetValueOrDefault() : DateTime.Now;
            this.Title = title;
        }
    }

    public class RecordPurchaseOrderEventCommandHandler : IRequestHandler<RecordPurchaseOrderEventCommand, bool>
    {
        private readonly ICurrentUserService _userService;
        private readonly TradefactDbContext _context;
        private readonly ILogger<RecordPurchaseOrderEventCommandHandler> _logger;

        public RecordPurchaseOrderEventCommandHandler(TradefactDbContext context, ICurrentUserService userService, ILogger<RecordPurchaseOrderEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<bool> Handle(RecordPurchaseOrderEventCommand command, CancellationToken cancellationToken)
        {
            var po_event = new PurchaseOrderEvent { EventType = command.EvenType, PurchaseOrderId = command.PurchaseOrderId};

            po_event.Description = po_event.EventType switch
            {
                PurchaseOrderEventType.Created => "Orders Created",
                PurchaseOrderEventType.Submitted => "Orders Submitted",
                PurchaseOrderEventType.Accepted => "Orders Accepted",
                PurchaseOrderEventType.Production => "Orders In Production",
                PurchaseOrderEventType.PreShipment => "Orders Pre-Shipment",
                PurchaseOrderEventType.Shipping => "Orders Shipped",
                PurchaseOrderEventType.Rejected => "Orders Rejected",
                PurchaseOrderEventType.Cancelled => "Orders Cancelled",
                PurchaseOrderEventType.Complete => "Orders Complete",
                PurchaseOrderEventType.Archived => "Orders Archived",
                PurchaseOrderEventType.DocumentUploaded => "Orders Document Uploaded",
                PurchaseOrderEventType.CommentAdded => "Comment Added",
                PurchaseOrderEventType.POEdit => "Orders Edited",
                PurchaseOrderEventType.EmailSent => command.Title,
                _ => "Orders Updated"
            };

            po_event.ActionedBy = new Guid(_userService.UserId);
            po_event.ActionDate = command.EventTime;


            try
            {
                _context.PurchaseOrderEvents.Add(po_event);
                _ = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
    }
}
