using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Tradefact.Portal.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private const string ProfileImage = "http://schemas.tradefact.com/2020/07/identity/claims/profileimage";

        public CurrentUserService()
        {
            UserId = Guid.NewGuid().ToString();
            Name = "Non-Registered User";
            Email = "unknown@tradefact.com";

        }

        public string UserId { get; }
        public string Name { get; }
        public string Email { get; }
        public string Avatar { get; }

        public bool IsAuthenticated { get; } = false;
    }
}
