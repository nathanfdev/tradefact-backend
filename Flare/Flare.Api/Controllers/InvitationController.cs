using Core.Common;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Flare.Api.Model;
using Flare.Api.Services.Certificate;
using Flare.Data;

namespace Flare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly AppSettings AppSettings;
        new private readonly FlareDbContext _context;

        public InvitationController(IOptions<AppSettings> appSettings, KeyVaultCertificateService keyVaultService, FlareDbContext context, IServiceBusClient serviceBusClient, UserManager<ApplicationUser> userManager = null) : base(context)
        {
            AppSettings = appSettings.Value;
            _userManager = userManager;
            _serviceBusClient = serviceBusClient;
            _context = context;
        }

        [HttpGet]
        [Route("CompanyNameFromHash")]
        public async Task<IActionResult> CompanyNameFromHash(string hash)
        {
            var partnerOrg = await _context.Organisations.FirstAsync(x => x.Id == Guid.Parse(Decrypt(hash)));
            return new OkObjectResult(new { OrganisationName = partnerOrg.Name });
        }

        [HttpPost]
        [Route(nameof(CreateInvitation))]
        public async Task<IActionResult> CreateInvitation([FromBody] InvitationRequest request)
        {

            //  Check does email exist;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.EmailAddress);
            if (user == null)
            {
                ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
                string loggedInUserEmail = identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault();
                var loggedInUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == loggedInUserEmail);

                Guid invitationId = Guid.NewGuid();
                Organisation loggedInOrg = await GetLoggedInOrganisation();

                await LogInvitation(request, loggedInOrg, loggedInUser, (request.InviteType == (int)InviteType.NewEmployee) ? loggedInOrg.Id : null);
                SendMessageToServiceBus(invitationId, request, loggedInOrg.Name);

                return new OkObjectResult(new { OrganisationName = request.CompanyName, EmailAlreadyExisting = false, PartnershipAlreadyExisting = true });
            }
            else
            {
                if (request.InviteType == (int)InviteType.NewEmployee) return Conflict();
                var org = _context.Organisations.First(x => x.Id == user.OrganisationId);
                if (user.Status == UserStatus.Active) return new OkObjectResult(new { OrganisationName = org.Name, EmailAlreadyExisting = true});
                else return Conflict();
            }
        }

        private async Task LogInvitation(InvitationRequest request, Organisation org, ApplicationUser user, Guid? memberOf)
        {
            InvitationLog ilog = new InvitationLog
            {
                // UserId = created_user.Id,
                EmailAddress = request.EmailAddress,
                GivenName = request.GivenName,
                CompanyName = request.CompanyName,
                InviteType = request.InviteType,
                MemberOfOrganisationId = memberOf,
                InviteRequestedByOrganisationId = org.Id,
                InviteRequestedByUserId = new Guid(user.Id)
            };

            _context.InvitationLog.Add(ilog);
            await _context.InvitationLog.AddAsync(ilog);
            _ = await _context.SaveChangesAsync();
        }

        private async Task<Organisation> GetLoggedInOrganisation()
        {
            return await _context.Organisations.FirstOrDefaultAsync(x => x.Id == this.OrganisationId);
        }

        [HttpPost]
        [Route("Resend")]
        public async Task<IActionResult> ResendInvitationLink([FromBody] ResendInvitationRequest request)
        {
            var original_invitation = await _context.InvitationLog.OrderByDescending(o=>o.CreationDateInternal).FirstOrDefaultAsync(q => q.EmailAddress == request.EmailAddress);
            if (original_invitation != null)
            {
                Guid invitationId = Guid.NewGuid();

                var loggedInOrg = await GetLoggedInOrganisation();

                InvitationRequest invitationRequest = new InvitationRequest
                {
                    GivenName = original_invitation.GivenName,
                    CompanyName = original_invitation.CompanyName,
                    EmailAddress = original_invitation.EmailAddress,
                    InviteType = original_invitation.InviteType
                };
                SendMessageToServiceBus(invitationId, invitationRequest, loggedInOrg.Name);

                return Ok();
            }
            return BadRequest();
        }

        private async void SendMessageToServiceBus(Guid invitationId, InvitationRequest request, string partnerName)
        {
            string link = new InviteLink
            {
                InviteCompany = partnerName,
                InviteEmail = request.EmailAddress,
                InviteId = invitationId.ToString(),
                InviteType = request.InviteType.ToString(),
                Issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/",
                B2CSignUpUrl = this.AppSettings.B2CSignUpUrl,
                B2CTenant = this.AppSettings.B2CTenant,
                B2CClientId = this.AppSettings.B2CClientId,
                B2CPolicy = this.AppSettings.B2CPolicy,
                B2CRedirectUri = this.AppSettings.B2CRedirectUri
            }.GetLink();

            string QueueName = "invitequeue";

            ServiceBusMessage<InviteMail> msg = new ServiceBusMessage<InviteMail>(new InviteMail
            {
                Email = request.EmailAddress,
                Partner = partnerName,
                OrganisationName = request.CompanyName,
                FirstName = request.GivenName,
                InviteType = (int)request.InviteType,
                Link = link,
                AlreadyRegistered = false
            });

            await _serviceBusClient.Publish<InviteMail>(msg, QueueName);
        }

        private async void SendMessageToServiceBusPartnershipRequest(string email, string name, string organisationName, string link)
        {

            string QueueName = "partnershiprequest";

            ServiceBusMessage<PartnershipRequest> msg = new ServiceBusMessage<PartnershipRequest>(new PartnershipRequest
            {
                Email = email,
                FirstName = name,
                OrganisationName = organisationName,
                Link = link
            });

            await _serviceBusClient.Publish<PartnershipRequest>(msg, QueueName);
        }

        private static string Crypt(string text)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));
        }

        private static string Decrypt(string text)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(text));
        }
    }
}