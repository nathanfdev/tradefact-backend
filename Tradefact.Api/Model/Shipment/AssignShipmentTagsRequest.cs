using Core.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Shipment
{
    [TypescriptAutoGeneration]
    public class AssignShipmentTagsRequest
    {
        public Guid ShipmentId { get; set; }
        public string[] Tags { get; set; }
    }

    public class AssignShipmentTagsRequestValidator : AbstractValidator<AssignShipmentTagsRequest>
    {
        public AssignShipmentTagsRequestValidator()
        {

            RuleFor(x => x.ShipmentId).NotEmpty();
        }
    }
}
