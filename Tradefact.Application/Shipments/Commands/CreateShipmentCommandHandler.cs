using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<CreateShipmentCommandHandler> _logger;
        private readonly IServiceBusClient _serviceBusClient;

        public CreateShipmentCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<CreateShipmentCommandHandler> logger, IServiceBusClient serviceBusClient)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceBusClient = serviceBusClient;
        }

        public async Task<Guid> Handle(CreateShipmentCommand message, CancellationToken cancellationToken)
        {
            QuotationRequest qr = await _mediator.Send(new GetQuotationRequestQuery { QuotationRequestId = message.QuotationRequestId, OrganisationId = message.OrganisationId, OrganisationType = message.OrganisationType});

            DateTime action_time = DateTime.Now;

            Shipment s = new Shipment();
            s.Id = Guid.NewGuid();

            s.FreightMovementId = qr.FreightMovementId;
            s.PartnershipId = qr.PartnershipId;
            s.QuotationRequestId = qr.Id;

            s.IncoTerms = qr.FreightMovement.IncoTerms;
            s.LoadType = qr.FreightMovement.LoadType;
            s.ShipmentType = qr.FreightMovement.ShipmentType;

            if (message.Route != null)
            {
                s.PortOfLoadingId = (!String.IsNullOrEmpty(message.Route.PortOfLoading.LocationCode)) ? message.Route.PortOfLoading.LocationCode : null;
                s.PortOfDischargeId = (!String.IsNullOrEmpty(message.Route.PortOfDischarge.LocationCode)) ? message.Route.PortOfDischarge.LocationCode : null;  
                s.ETD = message.Route.PortOfLoading.ETD.GetValueOrDefault();
                s.ETA = message.Route.PortOfDischarge.ETA.GetValueOrDefault();
                s.Route = JsonConvert.SerializeObject(message.Route);
            }
            s.SCAC = message.Route.CarrierCode;
            s.PlaceOfDispatchId = qr.FreightMovement.PlaceOfDispatchId;
            s.PlaceOfLoadingId = qr.FreightMovement.PlaceOfLoadingId;
            s.Notes = qr.FreightMovement.Notes;
            s.Tags = qr.FreightMovement.Tags;

            s.Booked = true;
            s.BookedDate = action_time;

            QuotationRequest quoteRequest = _context.QuotationRequests.Find(message.QuotationRequestId);
            quoteRequest.State = QuotationStateEnum.ACCEPTED;

            Quotation q = await  _context.Quotations.OrderByDescending(o => o.Revision).FirstOrDefaultAsync(q => q.QuotationRequestId == quoteRequest.Id);
            if (q != null && q.LoadType.HasValue)
            {
                s.LoadType = q.LoadType.GetValueOrDefault();
            }

            //Set all other Quoterequests for same FreightMovementId to Expired

            List<QuotationRequest> expired_notifications = 
                _context.QuotationRequests
                .Include(i=>i.FreightMovement)
                .Include(i => i.Partnership).ThenInclude(i => i.Provider)
                .Include(i => i.Partnership).ThenInclude(i => i.Client)
                .Where(x => x.FreightMovementId == quoteRequest.FreightMovementId  && x.Id != quoteRequest.Id).ToList();

            foreach (var quote_expired_notification in expired_notifications)
            {
                quote_expired_notification.State = QuotationStateEnum.EXPIRED;

                await SendMessageToServiceBus(new QuoteExpiredMail
                { 
                    Email = quote_expired_notification.Partnership.Provider.ContactEmail,
                    OrganisationName = quote_expired_notification.Partnership.Client.Name,
                    QuoteReceivedDate = quote_expired_notification.Submitted.Date.ToShortDateString(),
                    ShipmentName = quote_expired_notification.FreightMovement.Name,
                    TradefactId = quote_expired_notification.FreightMovement.Reference
                });
            }


            // Each booking is automatically assigned a collab room
            _context.Rooms.Add(new Room
            {
                Id = s.Id,
                IsOpen = true,
                UnreadCount = 0,
            });

            _logger.LogInformation("----- Creating Shipment - Shipment: {@Order}", s);
            _context.Shipments.Add(s);
            _ = await _context.SaveChangesAsync();

            return s.Id;
        }
        private async Task SendMessageToServiceBus(QuoteExpiredMail notification)
        {
            string QueueName = "quoteexpiredqueue";

            ServiceBusMessage<QuoteExpiredMail> msg = new ServiceBusMessage<QuoteExpiredMail>(notification);

            await _serviceBusClient.Publish<QuoteExpiredMail>(msg, QueueName);
        }
    }
}
