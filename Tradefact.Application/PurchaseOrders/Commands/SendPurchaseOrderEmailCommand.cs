using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.Models;
using Tradefact.Data;

namespace Tradefact.Application.PurchaseOrders.Commands
{
    public class SendPurchaseOrderEmailCommand : IRequest<bool>
    {
        public Guid? PurchaseOrderId { get; set; }

        public List<POContactRequest> Contacts { get; set; }

        public class SendPurchaseOrderEmailCommandHandler : IRequestHandler<SendPurchaseOrderEmailCommand, bool>
        {
            private readonly IMediator _mediator;
            private readonly TradefactDbContext _context;
            private readonly IDbConnection _connection;
            private readonly IServiceBusClient _serviceBusClient;

            public SendPurchaseOrderEmailCommandHandler(IDbConnection connection, TradefactDbContext context, IMediator mediator, IServiceBusClient serviceBusClient)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _context = context;
                _connection = connection;
                _serviceBusClient = serviceBusClient;
            }

            public async Task<bool> Handle(SendPurchaseOrderEmailCommand request, CancellationToken cancellationToken)
            {
                PurchaseOrder entity = await _context.PurchaseOrders.FindAsync(request.PurchaseOrderId.Value);
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Shipment), request.PurchaseOrderId);
                }

                List<PurchaseOrderRecipients> recipients = new List<PurchaseOrderRecipients>();

                var org = await _context.Organisations
                    .Include(i => i.Contacts).ThenInclude(contact => contact.Email)
                    .SingleOrDefaultAsync(q => q.Id == entity.SupplierId);

                B2BConnection connection = await _context.B2BConnections
                    .Include(i => i.Contacts).ThenInclude(i => i.Email)
                    .SingleOrDefaultAsync(q => q.LinkedOrganisationId == entity.SupplierId);

                // Supplier has contacts
                if (org.Contacts != null && org.Contacts.Count > 0)
                {
                    foreach (POContactRequest contactRequest in request.Contacts)
                    {
                        OrganisationContact contact = org.Contacts.FirstOrDefault(q => q.Id == contactRequest.Id);
                        if (contact != null && contact.Email.Count > 0)
                        {
                            PurchaseOrderRecipients c = new PurchaseOrderRecipients { FullName = contact.FullName ?? "Sir/Madam" };
                            bool isValidContact = false;
                            foreach (var email in contact.Email)
                            {
                                if (email != null && !String.IsNullOrEmpty(email.Email) && email.IsActive)
                                {
                                    c.Email = email.Email;
                                    c.RegistrationLink = contactRequest.RegistrationLink;
                                    isValidContact = true;

                                    if (email.IsDefault) break;
                                }
                            }
                            if (isValidContact)
                            {
                                recipients.Add(c);
                            }
                        }
                    }
                } else if (connection.Contacts != null && connection.Contacts.Count > 0)
                {
                    foreach (POContactRequest contactRequest in request.Contacts)
                    {
                        ConnectionContact contact = connection.Contacts.FirstOrDefault(q => q.Id == contactRequest.Id);
                        if (contact != null && contact.Email.Count > 0)
                        {
                            PurchaseOrderRecipients c = new PurchaseOrderRecipients { FullName = contact.FullName ?? "Sir/Madam" };
                            bool isValidContact = false;
                            foreach (var email in contact.Email)
                            {
                                if (email != null && !String.IsNullOrEmpty(email.Email) && email.IsActive)
                                {
                                    c.Email = email.Email;
                                    c.RegistrationLink = contactRequest.RegistrationLink;
                                    isValidContact = true;

                                    if (email.IsDefault) break;
                                }
                            }
                            if (isValidContact)
                            {
                                recipients.Add(c);
                            }
                        }
                    }
                }

                if (recipients.Count > 0)
                {
                    // At least one contact is valid to recieve email
                    
                    PurchaseOrderEmailSendRequest payload = new  PurchaseOrderEmailSendRequest
                    {
                        Recipients= recipients,
                        PurchaseOrderId = request.PurchaseOrderId.GetValueOrDefault(),
                    };

                    string QueueName = "pocreatedqueue";

                    ServiceBusMessage<PurchaseOrderEmailSendRequest> msg = new ServiceBusMessage<PurchaseOrderEmailSendRequest>(payload);
                    await _serviceBusClient.Publish<PurchaseOrderEmailSendRequest>(msg, QueueName);

                    foreach (var item in recipients)
                    {
                        _ = await _mediator.Send(new RecordPurchaseOrderEventCommand(entity.Id, PurchaseOrderEventType.EmailSent, DateTime.Now, $"Email sent to '{item.Email}'"));
                    }

                    return true;

                }
                return false;

            }
        }
    }
}
