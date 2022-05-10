using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;

namespace FunctionApp.Imports.Requests
{
    public class BookImportRequest
    {
        [JsonRequired]
        public string CompanyId { get; set; }

        [JsonRequired]
        public string ImportId { get; set; }

        public List<string> PurchaseOrders { get; set; } = new List<string>();
    }

    public class BookImportRequestValidator : AbstractValidator<BookImportRequest>
    {
        public BookImportRequestValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();

            RuleFor(x => x.ImportId).NotEmpty();
        }
    }
}