using Core.Enums;
using FluentValidation;
using Newtonsoft.Json;

namespace Tradfact.Api.Requests
{
    public class PartnershipInviteRequest
    {
        public string Email { get; set; }
        public PartnershipTypeEnum PartnershipType { get; set; }
        public bool IsProvider { get; set; }
    }
}