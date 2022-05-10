using System;

namespace Core.Models.External
{
    public class ApiKey
    {
        public string Key { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
