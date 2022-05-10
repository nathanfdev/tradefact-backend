using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using Tradefact.Api.Model;
using Tradefact.Api.Model.User;
using Tradefact.Data;
using Tradefact.Services;

namespace Tradefact.Api.Controllers
{
    public class UserController : FilesController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBillingService _billingService;

        public UserController(IBillingService billingService, ILogger<DocumentController> logger, TradefactDbContext context, IAzureBlobService blobService, IMimeMappingService mimeMappingService, UserManager<ApplicationUser> userManager = null) : base(logger, context, blobService, mimeMappingService)
        {
            _userManager = userManager;
            _billingService = billingService;
        }

        protected override string GetBlobPath()
        {
            return $"documents";
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(UserInfo), Description = "OK Result")]
        [Route(nameof(GetProfile))]
        public async Task<IActionResult> GetProfile()
        {
            UserInfo userinfo = await this.GetUserProfile();
            return new OkObjectResult(userinfo);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(BillingSubscription), Description = "OK Result")]
        [Route(nameof(GetSubscriptionStatus))]
        public async Task<IActionResult> GetSubscriptionStatus()
        {
            string subscriptionId = null;

            if (this.OrganisationId != null)
            {
                Organisation org = await _context.Organisations.SingleAsync(q => q.Id == this.OrganisationId);
                subscriptionId = org?.ChargebeeSubscriptionId;
            }

            BillingSubscription result = await _billingService.GetSubscriptionStatus(subscriptionId);
            return new OkObjectResult(result);
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

        public override string[] PermittedExtensions => this.PermittedImageExtensions;

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
                userinfo.OrganisationType = org.OrganisationTypeId switch
                {
                    OrganisationTypeEnum.SHIPPER => "SHIPPER",
                    OrganisationTypeEnum.PARTNER => "PARTNER",
                    OrganisationTypeEnum.BUYER => "BUYER",
                    OrganisationTypeEnum.SUPPLIER => "SUPPLIER",
                    _ => "NOTAUTHORISED"
                };

                ApplicationUser user = await _context.Users.Include(i => i.ProfileImage).Include(i => i.Address).Where(q => q.UserName == userinfo.Email).FirstOrDefaultAsync();
                if (user != null)
                {
                    userinfo.Roles = (user.IsAdmin) ? new List<String> { "Admin" } : new List<String> { "User" };
                    userinfo.Status = user.Status;
                    userinfo.Id = user.Id;

                    if (user.ProfileImage != null)
                    {
                        userinfo.Picture = user.ProfileImage.BlobUrl;
                    }
                    if (user.RegistrationDate.HasValue)
                    {
                        userinfo.RegistrationDate = DateTime.SpecifyKind(user.RegistrationDate.GetValueOrDefault(), DateTimeKind.Local);
                    }
                    
                    var defaultPreferences = "{\"dashboardWidgets\":[{\"key\": \"mapWidget\", \"width\":2,\"xPos\":0,\"yPos\":0}, {\"key\": \"exceptionsWidget\", \"width\":1,\"xPos\":2,\"yPos\":0}, {\"key\": \"newsWidget\", \"width\":1,\"xPos\":0,\"yPos\":1}]}";
                    var userPreferences = user.UserPreferences ?? defaultPreferences;
                    
                    userinfo.UserPreferences = JsonConvert.DeserializeObject(userPreferences);
                    
                    userinfo.HasCoreOrgData = org.Addresses.Count > 0 && org.Currency != null;

                    userinfo.GenericSKUEnabled = org.GenericSKUEnabled.GetValueOrDefault()  && org.OrganisationTypeId == OrganisationTypeEnum.SHIPPER; 
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

        [HttpPost]
        [SwaggerResponse("201", typeof(UserInfo), Description = "Created Result")]
        [DisableFormValueModelBinding]
        [Route(nameof(ProfileImage))]
        public async Task<IActionResult> ProfileImage()
        {
            UploadResult uploadResult = await this.ProcessUploadRequest(this.BlobPath);
            if (!uploadResult.Success)
            {
                ModelStateDictionary state = (ModelStateDictionary)uploadResult;
                return BadRequest(state);
            }
            Document profileimage = new Document { Id = Guid.NewGuid(), BlobUrl = uploadResult.BlobUrls[0], CompanyId = this.OrganisationId };
            _context.Documents.Add(profileimage);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string userId = identity.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            string email = identity.FindFirst(q => q.Type == "emails").Value;

            ApplicationUser user = await _context.Users.Where(q => q.Email ==email).FirstOrDefaultAsync();
            user.ProfileImageId = profileimage.Id;

            _context.SaveChanges();

            UserInfo userinfo = await this.GetUserProfile();
            return Created(nameof(UserController), userinfo);
        }


    }
}
