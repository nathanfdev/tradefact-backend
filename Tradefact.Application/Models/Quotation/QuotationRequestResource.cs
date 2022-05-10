using System;
using System.Text;

namespace Tradefact.Application.Models
{
    public class QuotationRequestResource
    {
        public Guid Id { get; set; }
        public int State { get; set; }
        public FreightMovementResource FreightMovement { get; set; }
        public QuotationResource Quotation { get; set; }
        public OrganisationResource Client { get; set; }
        public OrganisationResource Provider { get; set; }

        public int Revision { get; set; } = 0;
        public bool IsRevision => this.Revision > 1;
    }
}
