using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag.Annotations;
using Tradefact.AADB2C.Api.Model.Request;
using Tradefact.AADB2C.Api.Services.Certificate;
using Tradefact.Data;

namespace Tradefact.AADB2C.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private static Lazy<X509SigningCredentials> SigningCredentials;
        private readonly AppSettings AppSettings;
        private readonly TradefactDbContext _context;

        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisIsOurSecureSigningKey_$$$_NobodyWillEverGuessThis_%%%_!!"));


        public InvitationController(IOptions<AppSettings> appSettings, KeyVaultCertificateService keyVaultService, TradefactDbContext context, UserManager<ApplicationUser> userManager = null)
        {
            this.AppSettings = appSettings.Value;
            this._userManager = userManager;

            this._context = context;

            SigningCredentials = new Lazy<X509SigningCredentials>(() =>
            {
                var x509Certificate2Certs = keyVaultService.GetCertificateFromKeyVault().GetAwaiter().GetResult();
                return new X509SigningCredentials(x509Certificate2Certs);

                //using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                //{
                //    certStore.Open(OpenFlags.ReadOnly);
                //    var certCollection = certStore.Certificates.Find(
                //        X509FindType.FindByThumbprint,
                //        this.AppSettings.SigningCertThumbprint,
                //        false);

                //    // Get the first cert with the thumbprint
                //    if (certCollection.Count > 0)
                //    {
                //        return new X509SigningCredentials(certCollection[0]);
                //    }
                //}
                throw new Exception("Certificate not found");
            });
        }


        [HttpPost]
        [Route(nameof(CreateInvitation))]
        public async Task<IActionResult> CreateInvitation([FromBody] InvitationRequest request)
        {
            //if email already exists, send back error
            if (await _userManager.FindByEmailAsync(request.EmailAddress) == null || request.InviteType == (int)InviteType.Recovery)
            {
                Guid invitationId = Guid.NewGuid();
                Guid organisationId;
                Organisation loggedInOrg;

                IRegistration reg;
                if (request.InviteType == 1)
                {
                    reg = new ForwarderRegistration();
                } else
                {
                    reg = new ShipperRegistration();
                }
                reg.CompanyName = request.CompanyName;
                reg.EmailAddress = request.EmailAddress;
                reg.PersonName = request.PersonName;
                reg.Subscription = "essential";

                await LogInvitation(reg);
                string link = this.GetInviteLink(reg);

                return Ok();
            }
            else
            {
                //TODO: change this to be more descriptive
                return StatusCode(409);
            }
        }

        private async Task LogInvitation(IRegistration reg)
        {
            var inv = new InvitationLog
            {
                EmailAddress = reg.EmailAddress,
                GivenName = reg.PersonName,
                CompanyName = reg.CompanyName,
                SubscriptionPlan = reg.Subscription,
                InviteType = (int)reg.InviteType,
                InviteRequestedByOrganisationId = new Guid("4b60f7a0-7af7-43a9-82d6-fab7552369a2")
            };

            await _context.InvitationLog.AddAsync(inv);
            _ = await _context.SaveChangesAsync();
        }

        private string GetInviteLink(IRegistration reg)
        {
            string link = new InviteLink
            {
                InviteCompany = "Tradefact Logistics",
                InviteEmail = reg.EmailAddress,
                InviteId = Guid.NewGuid().ToString(),
                InviteType = reg.InviteType.ToString(),
                Issuer = "https://www.tradefact.com/",
                B2CSignUpUrl = this.AppSettings.B2CSignUpUrl,
                B2CTenant = this.AppSettings.B2CTenant,
                B2CClientId = this.AppSettings.B2CClientId,
                B2CPolicy = this.AppSettings.B2CPolicy,
                B2CRedirectUri = this.AppSettings.B2CRedirectUri
            }.GetLink();

            return link;
        }

        private async void SendMessageToServiceBus(Guid invitationId, InvitationRequest request, string partnerName)
        {
            string token = BuildIdToken(partnerName, request.EmailAddress, invitationId.ToString(), request.InviteType.ToString());
            string link = BuildUrl(token);
        }


        private async Task<Guid> AddNewOrganisation(InvitationRequest request)
        {
            Guid organisationId = Guid.NewGuid();
            var org = new Organisation
            {
                Id = organisationId,
                Name = request.CompanyName,
                ContactEmail = request.EmailAddress,
                PaymentTerms = 30,
                OrganisationTypeId = request.InviteType == (int)InviteType.NewFreightForwarder ? OrganisationTypeEnum.PARTNER : OrganisationTypeEnum.SHIPPER
            };

            await _context.Organisations.AddAsync(org);
            _ = await _context.SaveChangesAsync();
            return organisationId;
        }

        private async Task AddNewUser(Guid organisationId, InvitationRequest request)
        {
            await _userManager.CreateAsync(
                new ApplicationUser
                {
                    Email = request.EmailAddress,
                    UserName = request.EmailAddress,
                    EmailConfirmed = false,
                    OrganisationId = organisationId
                }
            );
            var created_user = await _userManager.FindByEmailAsync(request.EmailAddress);
            _ = await _userManager.AddClaimAsync(created_user, new Claim("OrganisationId", organisationId.ToString()));
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

        public enum InviteType
        {
            NewFreightForwarder = 1,
            NewShipper = 2,
            NewEmployee = 3,
            Recovery = 4
        }

        public interface IRegistration
        {
            string CompanyName { get; set; }
            string PersonName { get; set; }
            string EmailAddress { get; set; }
            string Subscription { get; set; }
            InviteType InviteType { get; }
        }

        public class Registration
        {
            public string CompanyName { get; set; }
            public string PersonName { get; set; }
            public string EmailAddress { get; set; }
            public string Subscription { get; set; }
        }

        public class ShipperRegistration : Registration, IRegistration
        {
            public InviteType InviteType => InviteType.NewShipper;
        }

        public class ForwarderRegistration : Registration, IRegistration
        {
            public InviteType InviteType => InviteType.NewFreightForwarder;
        }
    }
}