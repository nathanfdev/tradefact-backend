using Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models.Security
{
    public partial class User : Entity
    {
        public User()
        {
            ExternalLogins = new List<ExternalUserLoginInfo>();
        }

        /// <summary>
        /// Security account user name
        /// </summary>
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }

        public string DefaultLanguage { get; set; }

        public bool IsLockedOut
        {
            get
            {
                return LockoutEndDateUtc != null ? LockoutEndDateUtc.Value > DateTime.UtcNow : false;
            }
        }

        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }
        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        /// <summary>
        /// Returns true if user authenticated  returns false if it anonymous. 
        /// </summary>
        public bool IsRegisteredUser { get; set; }
        /// <summary>
        /// The flag indicates that the user is an administrator 
        /// </summary>
        public bool IsAdministrator { get; set; }
        public string UserType { get; set; }
        public AccountState UserState { get; set; }
        /// <summary>
        /// The user ID of an operator who has loggen in on behalf of a customer
        /// </summary>
        public string OperatorUserId { get; set; }
        /// <summary>
        /// The user name of an operator who has loggen in on behalf of a customer
        /// </summary>
        public string OperatorUserName { get; set; }

        public IList<ExternalUserLoginInfo> ExternalLogins { get; set; }

        //Selected and persisted currency code
        public string SelectedCurrencyCode { get; set; }

        /// <summary>
        /// Organisation id
        /// </summary>
        public Guid OrganisationId { get; set; }
        /// <summary>
        /// Organisation associated with user 
        /// </summary>
        public Organisation Contact { get; set; }

        /// <summary>
        /// All user permissions
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }

        /// <summary>
        /// Single user role
        /// </summary>
        //public Role Role
        //{
        //    get
        //    {
        //        return Roles?.FirstOrDefault();
        //    }
        //}
        ///// <summary>
        ///// All user roles
        ///// </summary>
        //public IEnumerable<Role> Roles { get; set; }

        ///// <summary>
        ///// All user orders
        ///// </summary>
        //[JsonIgnore]
        //[IgnoreDataMember]
        //public IMutablePagedList<CustomerOrder> Orders { get; set; }

        ///// <summary>
        ///// All user RFQ
        ///// </summary>
        //[JsonIgnore]
        //[IgnoreDataMember]
        //public IMutablePagedList<QuoteRequest> QuoteRequests { get; set; }

        ///// <summary>
        ///// All user subscriptions
        ///// </summary>
        //[JsonIgnore]
        //[IgnoreDataMember]
        //public IMutablePagedList<Subscription> Subscriptions { get; set; }

        //public IList<DynamicProperty> DynamicProperties => Contact?.DynamicProperties;

        //public string FirstName => Contact?.FirstName;
        //public string LastName => Contact?.LastName;
        //public string MiddleName => Contact?.MiddleName;
        //public string Name => Contact?.FullName;
        //public string TimeZone => Contact?.TimeZone;
        //public Address DefaultAddress => Organisation?.DefaultAddress;
        //public Address DefaultBillingAddress => Organisation?.DefaultBillingAddress;
        //public Address DefaultShippingAddress => Organisation?.DefaultShippingAddress;
        //public IList<Address> Addresses => Contact?.Addresses;
    }

}
