using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;

namespace Flare.Api.Infrastructure
{
    public class CurrentUserService : ICurrentUserService
    {
        private const string ProfileImage = "http://schemas.tradefact.com/2020/07/identity/claims/profileimage";

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("InternalUserId");
            Name = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            Email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            // Avatar = httpContextAccessor.HttpContext?.User?.FindFirstValue(ProfileImage);

            IsAuthenticated = UserId != null;
        }

        public string UserId { get; }
        public string Name { get; }
        public string Email { get; }
        public string Avatar { get; }

        public bool IsAuthenticated { get; }
    }
}
