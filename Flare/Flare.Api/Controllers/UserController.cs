using Core.Enums;
using Core.Models;
using Flare.Api.Model;
using Flare.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flare.Api.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(FlareDbContext context, UserManager<ApplicationUser> userManager = null) : base(context)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(UserInfo), Description = "OK Result")]
        [Route(nameof(GetProfile))]
        public async Task<IActionResult> GetProfile()
        {
            UserInfo userinfo = await this.GetUserProfile();
            return new OkObjectResult(userinfo);
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(UserInfo), Description = "OK Result")]
        [Route("{id:guid}/UpdatePreferences")]
        public async Task<IActionResult> UpdatePreferences(Guid id, [FromBody] object userPreferences)
        {
            var user = await _context.Users.SingleOrDefaultAsync(q => q.Id == id.ToString());
            user.UserPreferences = JsonConvert.SerializeObject(userPreferences);
            _ = await _context.SaveChangesAsync();

            UserInfo userinfo = await this.GetUserProfile();
            return new OkObjectResult(userinfo);
        }

        private async Task<UserInfo> GetUserProfile()
        {
            UserInfo userinfo = new UserInfo();

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

            userinfo.Email = identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault();
            userinfo.FullName = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            userinfo.FirstName = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Select(c => c.Value).SingleOrDefault();
            userinfo.LastName = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Select(c => c.Value).SingleOrDefault();

            // User must be attached to an organisation - otherwise attached app should receive a not fully active profile
            if (identity.HasClaim(q=> q.Type == "OrganisationId"))
            {

                Guid organisationId = new Guid(identity.Claims.Where(c => c.Type == "OrganisationId").Select(c => c.Value).SingleOrDefault());
                Organisation org = await _context.Organisations.Include(i => i.Addresses).SingleAsync(q => q.Id == this.OrganisationId);
                userinfo.Organisation = org.Name;
                userinfo.OrganisationId = org.Id;

                ApplicationUser user = await _context.Users.Include(i => i.Address).Where(q => q.UserName == userinfo.Email).FirstOrDefaultAsync();
                if (user != null)
                {
                    userinfo.Roles = (user.IsAdmin) ? new List<String> { "Admin" } : new List<String> { "User" };
                    userinfo.Status = user.Status;
                    userinfo.Id = user.Id;

                    if (user.RegistrationDate.HasValue)
                    {
                        userinfo.RegistrationDate = DateTime.SpecifyKind(user.RegistrationDate.GetValueOrDefault(), DateTimeKind.Local);
                    }
                    
                    var defaultPreferences = "{\"dashboardWidgets\":[{\"key\": \"mapWidget\", \"width\":2,\"xPos\":0,\"yPos\":0}, {\"key\": \"exceptionsWidget\", \"width\":1,\"xPos\":2,\"yPos\":0}, {\"key\": \"newsWidget\", \"width\":1,\"xPos\":0,\"yPos\":1}]}";
                    var userPreferences = user.UserPreferences ?? defaultPreferences;
                    
                    userinfo.UserPreferences = JsonConvert.DeserializeObject(userPreferences);
                    
                    userinfo.HasCoreOrgData = org.Addresses.Count > 0 && org.Currency != null;
                }
            }
            else
            {
                InvitationLog invitation = await _context.InvitationLog
                    .AsNoTracking()
                    .OrderByDescending(o => o.CreationDateInternal)
                    .FirstOrDefaultAsync(q => q.EmailAddress.ToLower() == userinfo.Email.ToLower() && q.IsActive);

                if (invitation?.ActivationStatus == AccountActivationStatus.Pending)
                {
                    userinfo.Status = UserStatus.Pending;
                }
                else
                {
                    userinfo.Status = UserStatus.NotFound;
                }
            }

            return userinfo;
        }
    }
}
