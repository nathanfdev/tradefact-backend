using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models
{
    public class OrganisationStatusResource
    {
        public bool ProfileComplete { get; set; } = false;
        public InboxStatusResource Inbox { get; set; }
    }

    public class InboxStatusResource
    {
        public int Orders { get; set; } = 0;
    }
}
