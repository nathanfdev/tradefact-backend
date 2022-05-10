using Core.Enums;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class AddConsignmentDetailsRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public LoadEnum ContainerRequirement { get; set; }

        [JsonRequired]
        public string DispatchMethod { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        [JsonRequired]
        public decimal Insurance { get; set; }

        [JsonRequired]
        public string PortOfDischarge { get; set; }

        [JsonRequired]
        public string PortOfLoading { get; set; }

        [JsonRequired]
        public string ShipmentType { get; set; }
    }

    public class AddConsignmentDetailsRequestValidator : AbstractValidator<AddConsignmentDetailsRequest>
    {
        public AddConsignmentDetailsRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();

            RuleFor(x => x.ContainerRequirement).NotEmpty();

            RuleFor(x => x.DispatchMethod).NotEmpty();

            RuleFor(x => x.Insurance).NotEmpty();

            RuleFor(x => x.PortOfDischarge).NotEmpty();

            RuleFor(x => x.PortOfLoading).NotEmpty();

            RuleFor(x => x.ShipmentType).NotEmpty();
        }
    }
}