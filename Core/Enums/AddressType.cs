using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Enums
{
    [Flags]
    public enum AddressType
    {
        Billing = 1,
        Shipping = 2,
        BillingAndShipping = Billing | Shipping
    }
}
