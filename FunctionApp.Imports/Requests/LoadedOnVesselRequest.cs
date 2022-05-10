using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class LoadedOnVesselRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }
    }

    public class LoadedOnVesselRequestValidator : AbstractValidator<LoadedOnVesselRequest>
    {
        public LoadedOnVesselRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}