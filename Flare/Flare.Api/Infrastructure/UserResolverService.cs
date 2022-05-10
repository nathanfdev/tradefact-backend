using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Flare.Api.Infrastructure
{
    public class UserResolverService: IUserResolverService
    {
        private readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUser()
        {
            var currentPrincipal = (ClaimsIdentity)_context.HttpContext.User?.Identity;
            if (currentPrincipal == null || !currentPrincipal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
            {
                return null;
            }

            if (currentPrincipal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
            {
                string userId = currentPrincipal.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                string email = currentPrincipal.FindFirst(q => q.Type == "emails").Value;

                return email;
            }
            return _context.HttpContext.User?.Identity?.Name;
        }
    }
}
