using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.CreateInvite.Model
{
    public interface IRegistration
    {
        string CompanyName { get; set; }
        string PersonName { get; set; }
        string EmailAddress { get; set; }
        string Subscription { get; set; }
        InviteType InviteType { get; }
    }

    public class Registration
    {
        public string CompanyName { get; set; }
        public string PersonName { get; set; }
        public string EmailAddress { get; set; }
        public string Subscription { get; set; }
    }

    public class ShipperRegistration : Registration, IRegistration
    {
        public InviteType InviteType => InviteType.NewShipper;
    }

    public class ForwarderRegistration : Registration, IRegistration
    {
        public InviteType InviteType => InviteType.NewFreightForwarder;
    }

}
