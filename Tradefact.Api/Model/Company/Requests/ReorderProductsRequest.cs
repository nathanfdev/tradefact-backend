using Core.Enums;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tradfact.Api.Requests
{
    public class ReorderProductsRequest
    {
        public string OrderReference { get; set; }
        public List<ProductsToOrder> ProductsToOrder { get; set; }
        public string CurrencyId { get; set; }
    }
}