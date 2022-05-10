using Core.Caching;
using Core.Common.Caching;
using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using Flare.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flare.Api.Infrastructure
{

    public static class ClaimsPrincipalExtensions
    {
        public static bool ClaimHasValue(this ClaimsIdentity ci, Predicate<Claim> match)
        {
            if (ci == null)
            {
                return false;
            }

            var claim = ci.FindFirst(match);
            return claim != null && !String.IsNullOrEmpty(claim.Value);
        }
    }

    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly FlareDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly int _UserCacheExpiryInMinutes;
        private readonly IServiceBusClient _serviceBusClient;
        //private readonly UserManager<ApplicationUser> _userManager;

        private const string ProfileImage = "http://schemas.tradefact.com/2020/07/identity/claims/profileimage";

        public ClaimsTransformer(IHttpContextAccessor httpContextAccessor, FlareDbContext context, IMemoryCache memoryCache, IOptions<AppSettings> appSettings, IServiceBusClient serviceBusClient)
        {
            _context = context;
            _memoryCache = memoryCache;
            this._UserCacheExpiryInMinutes = appSettings.Value.UserCacheMinutes;
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var currentPrincipal = (ClaimsIdentity)principal.Identity;//_principal.Identity;

                if (!currentPrincipal.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"))
                {
                    return await Task.FromResult(principal);
                }

                string userId = currentPrincipal.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                string email = currentPrincipal.FindFirst(q => q.Type == "emails").Value;

                var cacheKey = CacheKey.With(GetType(), $"TF_Auth_ID___{userId}");
                List<Claim> additional_claims = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
                {
                    cacheEntry = new MemoryCacheEntryOptions { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddMinutes(this._UserCacheExpiryInMinutes)) };

                    List<Claim> user_claims = new List<Claim>();

                    ApplicationUser active_user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(q => q.Email == email);
                    if (active_user != null)
                    {
                        if (!active_user.EmailConfirmed)
                        {
                            active_user = await _context.Users.FirstOrDefaultAsync(q => q.Email == email);

                            active_user.EmailConfirmed = true;

                            if (currentPrincipal.ClaimHasValue(c => c.Type == "name"))
                            {
                                active_user.FullName = currentPrincipal.FindFirst(q => q.Type == "name").Value;
                            }

                            active_user.Status = UserStatus.Active;
                            active_user.ExternalProverUUID = Guid.Parse(userId);
                            active_user.RegistrationDate = DateTime.UtcNow;

                            if (currentPrincipal.ClaimHasValue(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"))
                            {
                                active_user.GivenName = currentPrincipal.FindFirst(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
                            }
                            if (currentPrincipal.ClaimHasValue(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"))
                            {
                                active_user.Surname = currentPrincipal.FindFirst(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
                            }
                            _ = await _context.SaveChangesAsync();

                            active_user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(q => q.Email == email);
                        }

                        if (active_user.OrganisationId.HasValue)
                        {
                            user_claims.Add(new Claim("OrganisationId", active_user.OrganisationId.GetValueOrDefault().ToString()));
                            // Get the user type / current usage mode
                            Organisation org = await _context.Organisations.FindAsync(active_user.OrganisationId.GetValueOrDefault());
                            //OrganisationTypeEnum mode = org.OrganisationTypeId;
                            //user_claims.Add(new Claim("OrganisationType", ((int)mode).ToString()));
                        }
                        user_claims.Add(new Claim("InternalUserId", active_user.Id.ToString()));
                    }
                    else
                    {
                        // Initiate Account Setup on Tradefact - 30/03/2012 user account will not be present for new accounts.
                        InvitationLog invitation = await _context.InvitationLog.OrderByDescending(o=>o.CreationDateInternal).FirstOrDefaultAsync(q => q.EmailAddress.ToLower() == email.ToLower() && q.IsActive);
                        if (invitation != null)
                        {
                            if (!(invitation.ActivationStatus == AccountActivationStatus.Pending || 
                                invitation.ActivationStatus == AccountActivationStatus.Complete))
                            {
                                ServiceBusMessage<InvitationLog> msg = new ServiceBusMessage<InvitationLog>(invitation);
                                await _serviceBusClient.Publish<InvitationLog>(msg, "account-activate");

                                // Do not send new servicebus if one pending/active
                                invitation.LastActivationAttempt = DateTime.UtcNow;
                                invitation.ActivationStatus = AccountActivationStatus.Pending;
                                await _context.SaveChangesAsync();
                            }
                        }
                        active_user = new ApplicationUser { Status = active_user?.Status == UserStatus.Disabled ? UserStatus.Disabled : UserStatus.Pending };
                        active_user.FullName = active_user?.Status == UserStatus.Disabled ? "Disabled" : "Pending";

                        if (currentPrincipal.ClaimHasValue(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"))
                        {
                            active_user.GivenName = currentPrincipal.FindFirst(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
                        }
                        if (currentPrincipal.ClaimHasValue(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"))
                        {
                            active_user.Surname = currentPrincipal.FindFirst(q => q.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
                        }
                    }
                    user_claims.Add(new Claim(ClaimTypes.Email, email));
                    user_claims.Add(new Claim(ClaimTypes.Name, active_user.FullName ?? "NA"));

                    user_claims.Add(new Claim("UserStatus", active_user.Status switch
                    {
                        UserStatus.Disabled => "Disabled",
                        UserStatus.Pending => "Pending",
                        _ => "Active",
                    }));

                    return user_claims;
                });

                if (additional_claims.FirstOrDefault<Claim>(q => q.Type == "UserStatus").Value == "Pending")
                {
                    _memoryCache.Remove(cacheKey);
                }

                currentPrincipal.AddClaims(additional_claims);
            }

            return await Task.FromResult(principal);
        }

    }

}
