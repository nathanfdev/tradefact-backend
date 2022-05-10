using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tradefact.Data;
using FunctionApp.CreateInvite.Model;
using Core.Common;

namespace FunctionApp.CreateInvite
{
    public class Invite
    {
        private static readonly Guid envTradefactLogisticsOrgId = new Guid(Environment.GetEnvironmentVariable("TradefactLogisticsOrgId", EnvironmentVariableTarget.Process));
        private static readonly string envLogisticsPartnerName = Environment.GetEnvironmentVariable("LogisticsPartner", EnvironmentVariableTarget.Process);
        private static readonly string envInviteQueueName = Environment.GetEnvironmentVariable("Queue_Invite", EnvironmentVariableTarget.Process);
        private static readonly string envIssuer = Environment.GetEnvironmentVariable("Issuer");
        private static readonly string envB2CSignUpUrl = Environment.GetEnvironmentVariable("AazurAdB2C_SignUpUrl");
        private static readonly string envB2CTenant = Environment.GetEnvironmentVariable("AazurAdB2C_Tenant");
        private static readonly string envB2CPolicy = Environment.GetEnvironmentVariable("AazurAdB2C_Policy");
        private static readonly string envB2CClientId = Environment.GetEnvironmentVariable("AazurAdB2C_ClientId");
        private static readonly string envB2CRedirectUri = Environment.GetEnvironmentVariable("AazurAdB2C_RedirectUri");
        private static readonly string envAlreadyRegisteredUrl = Environment.GetEnvironmentVariable("AlreadyRegisteredUrl");

        protected readonly TradefactDbContext _context;

        public Invite(TradefactDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [FunctionName(nameof(InviteShipper))]
        public async Task<IActionResult> InviteShipper(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Shipper Registration HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IRegistration data = JsonConvert.DeserializeObject<ShipperRegistration>(requestBody);

            string invite_link = this.GetInviteLink(data);
            return new OkObjectResult(invite_link);
        }

        [FunctionName(nameof(InviteFreightForwarder))]
        public async Task<IActionResult> InviteFreightForwarder(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Shipper Registration HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IRegistration data = JsonConvert.DeserializeObject<ForwarderRegistration>(requestBody);

            string invite_link = this.GetInviteLink(data);
            return new OkObjectResult(invite_link);
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
                InviteRequestedByOrganisationId = envTradefactLogisticsOrgId
            };

            await _context.InvitationLog.AddAsync(inv);
            _ = await _context.SaveChangesAsync();
        }

        private string GetInviteLink(IRegistration reg)
        {
            string link =  new InviteLink
                {
                    InviteCompany = envLogisticsPartnerName,
                    InviteEmail = reg.EmailAddress,
                    InviteId = Guid.NewGuid().ToString(),
                    InviteType = reg.InviteType.ToString(),
                    Issuer = envIssuer,
                    B2CSignUpUrl = envB2CSignUpUrl,
                    B2CTenant = envB2CTenant,
                    B2CClientId = envB2CClientId,
                    B2CPolicy = envB2CPolicy,
                    B2CRedirectUri = envB2CRedirectUri
                }.GetLink();

            return link;
        }
    }
}
