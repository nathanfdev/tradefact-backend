using Core.Attributes;
using FluentValidation;
using System;

namespace Tradefact.Api.Model.Shipment
{
    [TypescriptAutoGeneration]
    public class AssignShipmentBOLRequest
    {
        public Guid ShipmentId { get; set; }
        public string BolNumber { get; set; }
    }

    public class AssignShipmentBOLRequestValidator : AbstractValidator<AssignShipmentBOLRequest>
    {
        public AssignShipmentBOLRequestValidator()
        {

            RuleFor(x => x.ShipmentId).NotEmpty();
            RuleFor(x => x.BolNumber).NotEmpty();
        }
    }
}
