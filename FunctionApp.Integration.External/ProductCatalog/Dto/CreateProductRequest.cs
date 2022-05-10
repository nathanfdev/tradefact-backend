using FluentValidation;
using FunctionApp.Integration.External.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FunctionApp.Integration.External.ProductCatalog
{

    public class CreateProductRequest : Resource<CreateProductRequest>, IKeyReferencable<CreateProductRequest>
    {
        public string Key => this.SKU;

        public string IsoCurrency { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
    }

    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 150);
            RuleFor(x => x.IsoCurrency).NotEmpty().MinimumLength(3).MaximumLength(3);

            //TODO: ensure this is valid based on ISO Codes.
        }
    }
}
