using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Tradefact.Data;

namespace Tradefact.UserManagement.Model
{
    public class ActivateAccountResult
    {
        public InvitationLog Invitation { get; set; }
        public Organisation Organisation { get; set; }
        public ApplicationUser ActiveUser { get; set; }
        public bool BillingSetupComplete { get; set; }
    }
}
