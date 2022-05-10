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
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Request;
using Tradefact.Api.Services.Certificate;
using Tradefact.Data;
using Tradfact.Api.Requests;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly AppSettings AppSettings;
        private readonly TradefactDbContext _context;
        private readonly ITradefactActivityService _tradefactActivityService;

        // private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisIsOurSecureSigningKey_$$$_NobodyWillEverGuessThis_%%%_!!"));
        // private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        // private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };


        public InvitationController(IOptions<AppSettings> appSettings, KeyVaultCertificateService keyVaultService, TradefactDbContext context, IServiceBusClient serviceBusClient, ITradefactActivityService tradefactActivityService, UserManager<ApplicationUser> userManager = null) : base(context)
        {
            AppSettings = appSettings.Value;
            _userManager = userManager;
            _serviceBusClient = serviceBusClient;
            _context = context;
            _tradefactActivityService = tradefactActivityService;
        }

        [HttpPost]
        [Route("SendPartnershipInvite")]
        public async Task<IActionResult> SendPartnershipInvite(PartnershipInviteRequest request)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string loggedInUser = identity.Claims.Where(c => c.Type == "emails").Select(c => c.Value).SingleOrDefault();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loggedInUser);


            var loggedInOrg = await _context.Organisations.FirstAsync(x => x.Id == this.OrganisationId);
            var encryptedGuid = Crypt(user.OrganisationId.ToString());

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string linkenv = "";
            if (env == "DEVELOPMENT" || env == "azure_dev") linkenv = "dev.app";
            else if (env == "azure_prd") linkenv = "app";

            StringBuilder link = new StringBuilder();
            link.Append($"https://{linkenv}.tradefact.com/#/");

            if (request.PartnershipType == PartnershipTypeEnum.LOGISTICS)
            {
                if (request.IsProvider) link.Append("directory/logistics");
                else link.Append("customers");
            }
            else if (request.PartnershipType == PartnershipTypeEnum.SUPPLY)
            {
                if (request.IsProvider) link.Append("directory/suppliers");
                else link.Append("directory/buyers");
            }

            link.Append($"?modalOpen=true&companyId={encryptedGuid}");

            if (request.PartnershipType == PartnershipTypeEnum.LOGISTICS) link.Append("&partnershipType=1");
            else if (request.PartnershipType == PartnershipTypeEnum.SUPPLY) link.Append("&partnershipType=2");

            if (request.IsProvider) link.Append("&isProvider=true");
            else link.Append("&isProvider=false");

            SendMessageToServiceBusPartnershipRequest(request.Email, user.FullName, loggedInOrg.Name, link.ToString());

            return Ok();
        }

        [HttpGet]
        [Route("CompanyNameFromHash")]
        public async Task<IActionResult> CompanyNameFromHash(string hash)
        {
            var partnerOrg = await _context.Organisations.FirstAsync(x => x.Id == Guid.Parse(Decrypt(hash)));
            return new OkObjectResult(new { OrganisationName = partnerOrg.Name });
        }

        [HttpPost]
        [Route("PartnershipAccepted")]
        public async Task<IActionResult> PartnershipAccepted(PartnershipCreateRequest request)
        {
            var loggedInOrg = await _context.Organisations.FirstAsync(x => x.Id == this.OrganisationId);
            var partnerOrg = await _context.Organisations.FirstAsync(x => x.Id == Guid.Parse(Decrypt(request.Hash)));

            Partnership partnership;

            if (request.IsProvider)
            {
                partnership = new Partnership
                {
                    ClientId = partnerOrg.Id,
                    ProviderId = loggedInOrg.Id,
                    PartnershipTypeId = request.PartnershipType
                };
            }
            else
            {
                partnership = new Partnership
                {
                    ClientId = loggedInOrg.Id,
                    ProviderId = partnerOrg.Id,
                    PartnershipTypeId = request.PartnershipType
                };
            }
            await _context.Partnerships.AddAsync(partnership);
            _ = await _context.SaveChangesAsync();

            return Ok();
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

                var eventName = (InviteType)request.InviteType switch
                {
                    InviteType.NewShipper =>
                        loggedInOrg.OrganisationTypeId == OrganisationTypeEnum.PARTNER ?
                            "customer_invited" : 
                            "company_invited",
                    InviteType.NewEmployee => "user_invited",
                    InviteType.NewFreightForwarderInvitedByShipper => "logistics_invited",
                    _ => "other_invited"
                };
                _ = _tradefactActivityService.TrackEvent(
                    User, 
                    loggedInOrg, 
                    eventName,
                    new TrackWith { Segment = true, Tradefact = false },
                    new EventProps { Segment = request }
                );

                return new OkObjectResult(new { OrganisationName = request.CompanyName, EmailAlreadyExisting = false, PartnershipAlreadyExisting = true });
            }
            else
            {
                if (request.InviteType == (int)InviteType.NewEmployee) return Conflict();
                var org = _context.Organisations.First(x => x.Id == user.OrganisationId);
                Partnership partnership;

                if(this.OrganisationType == OrganisationTypeEnum.PARTNER)
                {
                    partnership = _context.Partnerships.FirstOrDefault(x => x.ProviderId == this.OrganisationId && x.ClientId == org.Id);
                }
                else
                {
                    partnership = _context.Partnerships.FirstOrDefault(x => x.ClientId == this.OrganisationId && x.ProviderId == org.Id);
                }

                if (partnership != null) return new OkObjectResult(new { OrganisationName = org.Name, EmailAlreadyExisting = true, PartnershipAlreadyExisting = true });
                if (user.Status == UserStatus.Active) return new OkObjectResult(new { OrganisationName = org.Name, EmailAlreadyExisting = true, PartnershipAlreadyExisting = false });
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

                await _tradefactActivityService.TrackEvent(
                    User,
                    this.OrganisationId,
                    "invite_resent",
                    new TrackWith { Segment = false, Tradefact = true },
                    new EventProps { Tradefact = new TradefactEventProps { Reference = original_invitation.EmailAddress, Type = ActivityTypeEnum.INVITE, Entity = ActivityEntityTypeEnum.NETWORK, CustomDescription = "Invite Resent" } }
                );

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