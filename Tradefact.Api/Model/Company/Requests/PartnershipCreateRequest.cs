using Core.Enums;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class PartnershipCreateRequest
    {
        public string Hash { get; set; }
        public PartnershipTypeEnum PartnershipType { get; set; }
        public bool IsProvider { get; set; }
    }
}