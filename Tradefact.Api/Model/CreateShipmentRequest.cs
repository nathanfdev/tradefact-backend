using Core.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model
{
    public class CreateShipmentRequest
    {
        public Guid QuotationRequestId { get; set; }
        public RouteSchedule Route { get; set; }
    }

    public class AmendShipmentScheduleRequest
    {
        public Guid ShipmentId { get; set; }
        public Decimal? Charge { get; set; }
        public DateTimeOffset GoodsReady { get; set; }
        public DateTimeOffset? DeparturelDate { get; set; }
        public DateTimeOffset? ArrivalDate { get; set; }
        public RouteSchedule Route { get; set; }
        public int? NumberOfChanges { get; set; }
        public bool IsManualRoute { get; set; }
        public bool IsReSchedule { get; set; } = false;
    }

    public class AmendShipmentScheduleRequestValidator : AbstractValidator<AmendShipmentScheduleRequest>
    {
        public AmendShipmentScheduleRequestValidator()
        {
            RuleFor(x => x.Charge).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.IsManualRoute).NotNull();

            RuleFor(x => x.GoodsReady)
                .NotEmpty().WithMessage("Required field");

            RuleFor(x => x.Route).NotNull();

            When(x => x.IsManualRoute, () => {

                RuleFor(x => x.DeparturelDate)
                    .NotEmpty().WithMessage("Required field")
                    .GreaterThanOrEqualTo(p => p.GoodsReady).WithMessage("Departure date must be on or after goods ready date")
                    .GreaterThanOrEqualTo(p => DateTime.Now.ToUniversalTime().Date).WithMessage("Departure date must not be in the past");

                RuleFor(x => x.ArrivalDate)
                    .NotEmpty().WithMessage("Required field")
                    .GreaterThanOrEqualTo(p => p.DeparturelDate).WithMessage("Departure date must not be in the past");

                RuleFor(x => x.NumberOfChanges).GreaterThanOrEqualTo(0);
            });
        }
    }
}
