using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tradefact.Portal.Models;
using Tradefact.Portal.Service;
using Tradefact.Data;

namespace Tradefact.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TradefactDbContext _context;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly AppSettings AppSettings;

        public HomeController(IOptions<AppSettings> appSettings, ILogger<HomeController> logger, TradefactDbContext context, IServiceBusClient serviceBusClient, UserManager<ApplicationUser> userManager = null)//, IHttpService httpService, IConfiguration config)
        {
            this.AppSettings = appSettings.Value;
            _logger = logger;
            _context = context;
            _serviceBusClient = serviceBusClient;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult SendNewInvite()
        {
            return View(new SendNewInviteModel());
        }

        [HttpGet]
        public IActionResult CreateRelationship()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendNewInvite(string email, string companyName, string firstName, string inviteType, string sentBy, string sendEmail)
        {
            string url;
            SendNewInviteModel model = new SendNewInviteModel();
            var request = new SendInviteRequest()
            {
                CompanyName = companyName,
                Email = email,
                GivenName = firstName,
                InviteType = Convert.ToInt32(inviteType)
            };

            //if email already exists, send back error
            if (await _userManager.FindByEmailAsync(request.Email) == null)
            {
                Guid invitationId = Guid.NewGuid();
                Guid organisationId;

                switch (request.InviteType)
                {
                    case (int)InviteType.NewFreightForwarder:
                        organisationId = await AddNewOrganisation(request.Email, request.CompanyName, request.GivenName);
                        await AddNewUser(organisationId, request.Email, request.GivenName, 1, request.CompanyName);
                        if (sendEmail == "1") SendMessageToServiceBus(invitationId, request.Email, request.CompanyName, request.CompanyName, sentBy ?? "Tradefact", request.InviteType.ToString());
                        else
                        {
                            string token = BuildIdToken(sentBy, email, invitationId.ToString(), request.InviteType.ToString());
                            model.Link = BuildUrl(token);
                        }   
                        break;
                    case (int)InviteType.NewShipper:
                        break;
                    case (int)InviteType.NewEmployee:
                        break;
                    default:
                        //This can be empty, as the request will get bounced if it's outside of 1-3
                        break;
                }



                model.ResultMessage = "Sent invite successfully";
                return View(model);

            }

            model.ResultMessage = "Email exists already, please choose a different one. Invite was not sent.";

            return View(model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task<Guid> AddNewOrganisation(string email, string companyName, string givenName)
        {
            Guid organisationId = Guid.NewGuid();
            var org = new Organisation
            {
                Id = organisationId,
                Name = companyName,
                ContactEmail = email,
                ContactName = givenName,
                PaymentTerms = 30,
                OrganisationTypeId = OrganisationTypeEnum.PARTNER
            };

            await _context.Organisations.AddAsync(org);
            _ = await _context.SaveChangesAsync();
            return organisationId;
        }

        //private async Task AddNewPartnership(Guid organisationId)
        //{
        //    var partnership = new Partnership
        //    {
        //        ClientId = organisationId,
        //        ProviderId = this.OrganisationId,
        //        PartnershipTypeId = PartnershipTypeEnum.LOGISTICS

        //    };
        //    await _context.Partnerships.AddAsync(partnership);
        //    _ = await _context.SaveChangesAsync();
        //}

        private async Task AddNewUser(Guid organisationId, string email, string name, int inviteType, string companyName)
        {
            await _userManager.CreateAsync(
                new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    FullName = name,
                    EmailConfirmed = false,
                    OrganisationId = organisationId
                }
            );
            var created_user = await _userManager.FindByEmailAsync(email);
            _ = await _userManager.AddClaimAsync(created_user, new Claim("OrganisationId", organisationId.ToString()));

            InvitationLog ilog = new InvitationLog
            {
                UserId = created_user.Id,
                EmailAddress = email,
                GivenName = name,
                CompanyName = companyName,
                InviteType = inviteType
            };

            _context.InvitationLog.Add(ilog);
            await _context.InvitationLog.AddAsync(ilog);
            _ = await _context.SaveChangesAsync();
        }

        private async void SendMessageToServiceBus(Guid invitationId, string email, string companyName, string name, string partnerName, string inviteType)
        {
            string token = BuildIdToken(partnerName, email, invitationId.ToString(), inviteType);
            string link = BuildUrl(token);

            string QueueName = "invitequeue";

            ServiceBusMessage<InviteMail> msg = new ServiceBusMessage<InviteMail>(new InviteMail
            {
                Email = email,
                Partner = partnerName,
                OrganisationName = companyName,
                FirstName = name,
                InviteType = 1,
                Link = link
            });

            await _serviceBusClient.Publish<InviteMail>(msg, QueueName);
        }

        private string BuildIdToken(string companyname, string emailaddress, string uniqueid, string invitationType)
        {
            string issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";

            var securityKey = Encoding.UTF8.GetBytes("2VK62QTn0m1hMcn0DQ3TRADArF4ct6yIiSvYgdRwjZtU5QhI=");

            var signingKey = new SymmetricSecurityKey(securityKey);
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim("invite.email", emailaddress, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.id", uniqueid, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.company", companyname, System.Security.Claims.ClaimValueTypes.String, issuer));
            claims.Add(new System.Security.Claims.Claim("invite.type", invitationType, System.Security.Claims.ClaimValueTypes.String, issuer));

            // var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512);

            // Create the token
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer,
                    this.AppSettings.B2CClientId,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    signingCredentials);

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }

        private string BuildUrl(string token)
        {
            string nonce = Guid.NewGuid().ToString("n");

            return string.Format(this.AppSettings.B2CSignUpUrl,
                    this.AppSettings.B2CTenant,
                    this.AppSettings.B2CPolicy,
                    this.AppSettings.B2CClientId,
                    Uri.EscapeDataString(this.AppSettings.B2CRedirectUri),
                    nonce) + "&id_token_hint=" + token;
        }
    }
}
