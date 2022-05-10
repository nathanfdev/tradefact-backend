using Flare.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Tradefact.Application.Models;

namespace Flare.Api.Model
{
    /// <summary>
    /// Represents user information returned from a standard OpenID Connect `/userinfo` endpoint.
    /// </summary>
    /// <remarks>
    /// See http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims for more details.
    /// </remarks>
    public class UserResource
    {
        /// <summary>
        /// Subject-Identifier for the user at the issuer. A unique value to identify the user.
        /// </summary>
        [JsonProperty("Id")]
        public string Id { get; set; }


        /// <summary>
        /// Organisation for the user. A unique value to identify the user.
        /// </summary>
        [JsonProperty("role")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Full name of the user in displayable form including all name parts, 
        /// possibly including titles and suffixes, ordered according to the user's locale and preferences.
        /// </summary>
        [JsonProperty("fullname")]
        public string FullName { get; set; }

        /// <summary>
        /// Given name(s) or first name(s) of the user. 
        /// </summary>
        /// <remarks>
        /// May contain multiple given names separated by space characters.
        /// </remarks>
        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        /// <summary>
        /// Surname(s) or last name(s) of the user. 
        /// </summary>
        /// <remarks>
        /// May contain multiple given names separated by space characters or no family name at all.
        /// </remarks>
        [JsonProperty("familyName")]
        public string Surname { get; set; }

        /// <summary>
        /// URL of the user's profile picture.
        /// </summary>
        /// <remarks>
        /// Note that this URL SHOULD specifically reference a profile photo of the End-User suitable for 
        /// displaying when describing the user, rather than an arbitrary photo taken by the user.
        /// </remarks>
        [JsonProperty("picture")]
        public string Picture { get; set; }

        /// <summary>
        /// User's preferred e-mail address. 
        /// </summary>
        /// <remarks>
        /// You MUST NOT rely upon this value being unique.
        /// </remarks>
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Time and date the user's information was last updated. 
        /// </summary>
        [JsonProperty("status")]
        public UserStatus Status { get; set; }


        [JsonProperty("locationId")]
        public Guid? LocationId { get; set; }


        [JsonProperty("address")]
        public AddressResource Address { get; set; }

        [JsonProperty("isExposedToOtherOrganization")]
        public bool IsExposedToOtherOrganization { get; set; }

        /// <summary>
        /// Additional claims about the user.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalClaims { get; set; }

    }

}
