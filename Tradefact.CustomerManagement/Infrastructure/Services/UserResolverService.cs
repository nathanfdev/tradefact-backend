using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Infrastructure.Services
{
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUser()
        {
            return "Admin";

            //var currentPrincipal = (ClaimsIdentity)_context.HttpContext.User?.Identity;
            //if (currentPrincipal == null || !currentPrincipal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
            //{
            //    return null;
            //}

            //if (currentPrincipal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
            //{
            //    string userId = currentPrincipal.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            //    string email = currentPrincipal.FindFirst(q => q.Type == "emails").Value;

            //    return email;
            //}
            //return _context.HttpContext.User?.Identity?.Name;
        }
    }
}
