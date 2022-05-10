using Core.Enums;
using Core.Models;
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
    public class AmendShipmentScheduleCommandHandler : IRequestHandler<AmendShipmentScheduleCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<AmendShipmentScheduleCommandHandler> _logger;

        public AmendShipmentScheduleCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<AmendShipmentScheduleCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(AmendShipmentScheduleCommand message, CancellationToken cancellationToken)
        {
            DateTime action_time = DateTime.UtcNow;

            Shipment s = await _context.Shipments.SingleOrDefaultAsync(q => q.Id == message.ShipmentId);
            FreightMovement fm = await _context.FreightMovements.SingleOrDefaultAsync(q => q.Id == s.FreightMovementId);

            if (message.IsManualRoute)
            {
                if(s.ShipmentType == ShipmentTypeEnum.SEA)
                {
                    Carrier carrier = _context.Carriers.Find(message.Route.CarrierCode);
                    message.Route.CarrierName = carrier.Name;
                }

                Location pol = new Location { };
                Location pod = new Location { };

                if (!String.IsNullOrEmpty(message.Route.PortOfLoading?.LocationCode))
                {
                    pol = _context.Locations.FirstOrDefault(q => q.LocCode == message.Route.PortOfLoading.LocationCode);
                }
                if (!String.IsNullOrEmpty(message.Route.PortOfDischarge?.LocationCode))
                {
                    pod = _context.Locations.FirstOrDefault(q => q.LocCode == message.Route.PortOfDischarge.LocationCode);
                }

                DateTimeOffset offset_etd = message.DeparturelDate.GetValueOrDefault();
                DateTime? etd = offset_etd.DateTime;
                message.Route.PortOfLoading = new WayPoint
                {
                    LocationCode = pol?.LocCode,
                    ETD = etd,
                    FullName = pol?.Name,
                    Name = pol?.Name
                };

                DateTimeOffset offset_eta = message.ArrivalDate.GetValueOrDefault();
                DateTime? eta = offset_eta.DateTime;
                message.Route.PortOfDischarge = new WayPoint
                {
                    LocationCode = pod?.LocCode,
                    ETA = eta,
                    FullName = pod?.Name,
                    Name = pod?.Name
                };
            }

            if (message.Route != null)
            {
                s.PortOfLoadingId = message.Route.PortOfLoading.LocationCode;
                fm.PortOfLoadingId = message.Route.PortOfLoading.LocationCode;
                s.PortOfDischargeId = message.Route.PortOfDischarge.LocationCode;
                fm.PortOfDischargeId = message.Route.PortOfDischarge.LocationCode;
                s.ETD = message.Route.PortOfLoading.ETD.GetValueOrDefault();
                s.ETA = message.Route.PortOfDischarge.ETA.GetValueOrDefault();
                s.Route = JsonConvert.SerializeObject(message.Route);
                s.SCAC = message.Route.CarrierCode;
            }
            DateTimeOffset offset_goodsready = message.GoodsReady;
            fm.GoodsReady = offset_goodsready.DateTime;

            string charge_desc = "Adjustment";
            if (message.IsReSchedule)
            {
                s.IsRescheduled = true;
                s.Rescheduled += 1;
                s.LastRescheduleTime = action_time;

                charge_desc = "Reschedule";
            }
            if (message.Charge.GetValueOrDefault() != 0)
            {
                Quotation q = await _context.Quotations
                    .Include(i => i.OriginCharges)
                    .Include(i => i.FreightCharges)
                    .Include(i => i.DestinationCharges)
                    .Include(i => i.AdditionalCharges)
                    .FirstOrDefaultAsync(q => q.QuotationRequestId == s.QuotationRequestId);

                int index = q.AdditionalCharges.Any() ? q.AdditionalCharges.Max(s => s.Seq) : 0;
                AdditionalCharge additional_charge = new AdditionalCharge(charge_desc, 1, message.Charge.GetValueOrDefault(), 0, 0, 0, q.CurrencyId ?? q.CurrencyId, 1, index + 1);

                q.AdditionalCharges.Add(additional_charge);

                q.BaseCurrency = new Core.Models.Total() { CurrencyId = q.BaseCurrency.CurrencyId, DiscountAmount = 0, NetAmount = 0, TaxAmount = 0, TotalAmount = 0 };
                q.TotalQuantity = 0;
                foreach (OriginCharge c in q.OriginCharges)
                {
                    q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                    q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                    q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                    q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                    q.TotalQuantity += c.Quantity;
                }
                foreach (FreightCharge c in q.FreightCharges)
                {
                    q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                    q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                    q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                    q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                    q.TotalQuantity += c.Quantity;
                }
                foreach (DestinationCharge c in q.DestinationCharges)
                {
                    q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                    q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                    q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                    q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                    q.TotalQuantity += c.Quantity;
                }
                foreach (AdditionalCharge c in q.AdditionalCharges)
                {
                    q.BaseCurrency.NetAmount = q.BaseCurrency.NetAmount + c.BaseCurrency.NetAmount;
                    q.BaseCurrency.TaxAmount = q.BaseCurrency.TaxAmount + c.BaseCurrency.TaxAmount;
                    q.BaseCurrency.DiscountAmount = q.BaseCurrency.DiscountAmount + c.BaseCurrency.DiscountAmount;
                    q.BaseCurrency.TotalAmount = q.BaseCurrency.TotalAmount + c.BaseCurrency.TotalAmount;
                    q.TotalQuantity += c.Quantity;
                }
                q.Total = new Core.Models.Total() { CurrencyId = q.BaseCurrency.CurrencyId, DiscountAmount = q.BaseCurrency.DiscountAmount, NetAmount = q.BaseCurrency.NetAmount, TaxAmount = q.BaseCurrency.TaxAmount, TotalAmount = q.BaseCurrency.TotalAmount };
            }

            _logger.LogInformation("----- Creating Shipment - Shipment: {@Order}", s);
            _ = await _context.SaveChangesAsync();
            return s.Id;
        }
    }
}
