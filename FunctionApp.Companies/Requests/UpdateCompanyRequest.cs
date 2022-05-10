
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Companies.Requests
{
    public class UpdateCompanyRequest : CosmosItem<UpdateCompanyRequest>
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string Name { get; set; }
    }

    public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.ETag).NotEmpty();
        }
    }
}