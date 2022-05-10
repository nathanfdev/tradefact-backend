using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tradefact.Api.Infrastructure.Extensions
{
    public static class PrincipalExtension
    {
        /// <summary>
        /// Retrieves the Firm Id claim if it exists
        /// </summary>
        public static Guid GetUserOrganisationId(this IPrincipal principal)
        {
            return Guid.TryParse(principal.FindFirstOrEmpty("OrganisationId"), out var organisationId)
                ? organisationId
                : default(Guid);
        }
        /// <summary>
        /// Retrieves the first claim that is matched by the 
        /// specified type if it exists, String.Empty otherwise.
        /// </summary>
        public static string FindFirstOrEmpty(this IPrincipal principal, string type)
        {
            return principal is ClaimsPrincipal p
                ? p.FindFirst(type)?.Value ?? string.Empty
                : string.Empty;
        }
    }
}
