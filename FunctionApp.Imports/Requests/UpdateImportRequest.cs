using Core.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class UpdateImportRequest : CosmosItem<UpdateImportRequest>
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        [JsonRequired]
        public string Name { get; set; }
    }

    public class UpdateImportRequestValidator : AbstractValidator<UpdateImportRequest>
    {
        public UpdateImportRequestValidator()
        {

            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.ImportId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.ETag).NotEmpty();
        }
    }
}