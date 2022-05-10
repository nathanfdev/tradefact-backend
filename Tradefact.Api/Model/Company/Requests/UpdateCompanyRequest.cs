
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class UpdateCompanyRequest //: CosmosItem<UpdateCompanyRequest>
    {
        public string Id { get; set; }
        [JsonRequired]
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephone { get; set; }
        public string Currency { get; set; }
        public int PaymentTerms { get; set; }
        public string TaxId { get; set; }
        // public UpdateAddressRequest InvoiceAddress { get; set; }
    }

    public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.PaymentTerms).InclusiveBetween(1, 60); ;
        }
    }
}