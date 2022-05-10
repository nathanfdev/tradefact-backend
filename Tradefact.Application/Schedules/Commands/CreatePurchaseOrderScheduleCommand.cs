using Core.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Data;

namespace Tradefact.Application.Schedules.Commands
{
    [DataContract]
    public class CreatePurchaseOrderScheduleCommand
    : IRequest<Guid>
    {
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public Guid PurchaseOrderId { get; private set; }

        [DataMember]
        public Guid PlaceOfLoading { get; private set; }

        [DataMember]
        public DateTime GoodsReadyDate { get; private set; }

        [DataMember]
        public List<CreateScheduleLineRequest> ScheduleLines { get; private set; }

        [DataMember]
        [JsonIgnore]
        public Guid OrganisationId { get; set; }
    }

    public class CreatePurchaseOrderScheduleCommandHandler
            : IRequestHandler<CreatePurchaseOrderScheduleCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<CreatePurchaseOrderScheduleCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public CreatePurchaseOrderScheduleCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<CreatePurchaseOrderScheduleCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreatePurchaseOrderScheduleCommand command, CancellationToken cancellationToken)
        {
            var existingPurchaseOrder = await _context.PurchaseOrders
                .Where(c => c.Id == command.PurchaseOrderId && c.CompanyId == command.OrganisationId)
                .Select(s => new { s.Id, s.IsLocked } )
                .FirstOrDefaultAsync();

            if (existingPurchaseOrder == null) throw new NotFoundException("PurchaseOrder", command.PurchaseOrderId);

            Guid scheduleId = Guid.NewGuid();

            Address placeOfLoading = _context.Addresses.Find(command.PlaceOfLoading);
            foreach (var sc in command.ScheduleLines)
            {
                PurchaseOrderItemScheduleLine line = new PurchaseOrderItemScheduleLine()
                {
                    Id = scheduleId,
                    Description = command.Name,
                    PurchaseOrderId = command.PurchaseOrderId,
                    PurchaseOrderItemId = sc.PurchaseOrderItemId,
                    PlaceOfLoadingId = command.PlaceOfLoading,
                    CountryofLoadingCode = placeOfLoading.CountryCode,
                    ScheduleLineCommittedQuantity = sc.ScheduleLineCommittedQuantity,
                    RequestedDeliveryDate = command.GoodsReadyDate,
                    ConfirmedGoodsReadyDate = command.GoodsReadyDate
                };
                _context.PurchaseOrderItemScheduleLines.Add(line);
            }

            //  When PO is marked as shipped we should always lock the order
            if (!existingPurchaseOrder.IsLocked)
            {
                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == command.PurchaseOrderId && c.CompanyId == command.OrganisationId);
                po.IsLocked = true;
            }
            _ = await _context.SaveChangesAsync();

            return scheduleId;
        }
    }

    public class CreateScheduleLineRequest
    {
        [DataMember]
        [JsonProperty("PurchaseOrderItemId")]
        public Guid PurchaseOrderItemId { get; set; }

        [JsonProperty("ScheduleLineCommittedQuantity")]
        public decimal ScheduleLineCommittedQuantity { get; set; }
    }

    public class CreatePurchaseOrderScheduleCommandValidator : AbstractValidator<CreatePurchaseOrderScheduleCommand>
    {
        public CreatePurchaseOrderScheduleCommandValidator()
        {
            RuleFor(x => x.PurchaseOrderId).NotNull().NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.PlaceOfLoading).NotNull().NotEmpty();

            RuleFor(x => x.GoodsReadyDate)
            .NotEmpty().WithMessage("Goods Ready Date Required")
            .LessThan(p => DateTime.Now).WithMessage("a data deve estar no passado");

            RuleFor(x => x.ScheduleLines).NotNull().WithMessage("ScheduleLine collectionis required.");
        }
    }
}
