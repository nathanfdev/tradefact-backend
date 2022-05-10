using Core.Common;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Data;
using Tradefact.UserManagement.Model;

namespace Tradefact.UserManagement.Functions.Register
{
    class ShipperRegistration
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
        protected readonly IServiceBusClient _serviceBusClient;
        protected readonly ITradefactActivityService _tradefactActivityService;

        public ShipperRegistration(TradefactDbContext context, IServiceBusClient serviceBusClient, ITradefactActivityService tradefactActivityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            _tradefactActivityService = tradefactActivityService;
        }

        [FunctionName(nameof(DoRegistration))]
        public async Task DoRegistration(
            [ServiceBusTrigger("%Queue_ShipperRegistration%", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            ILogger log)
        {
            string inputMessage = Encoding.UTF8.GetString(message.Body);
            Model.Registration reg = JsonConvert.DeserializeObject<Model.Registration>(inputMessage);

            log.LogInformation($"message - " + inputMessage);

            ApplicationUser existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(q => q.Email.ToLower() == reg.EmailAddress.ToLower());
            if (existingUser is null)
            {
                await LogInvitation(reg);
                await SendInviteMail(reg, false);
            }
            else
            {
                await SendInviteMail(reg, true); // Send already registered email
            }
        }

        private async Task LogInvitation(Registration reg)
        {
            var inv = new InvitationLog
            {
                Id = Guid.NewGuid(),
                EmailAddress = reg.EmailAddress,
                GivenName = reg.PersonName,
                CompanyName = reg.CompanyName,
                SubscriptionPlan = reg.Subscription,
                InviteType = (int)InviteType.NewShipper,
                InviteRequestedByOrganisationId = envTradefactLogisticsOrgId
            };

            await _context.InvitationLog.AddAsync(inv);
            _ = await _context.SaveChangesAsync();

            await _tradefactActivityService.TrackAnonymousEvent(inv.Id, inv.GivenName, inv.EmailAddress, inv.CompanyName, "invitation_sent", new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = inv });
        }

        private async Task SendInviteMail(Registration reg, bool alreadyRegistered)
        {
            string link = alreadyRegistered ? envAlreadyRegisteredUrl :
                new InviteLink
                {
                    InviteCompany = envLogisticsPartnerName,
                    InviteEmail = reg.EmailAddress,
                    InviteId = Guid.NewGuid().ToString(),
                    InviteType = InviteType.NewShipper.ToString(),
                    Issuer = envIssuer,
                    B2CSignUpUrl = envB2CSignUpUrl,
                    B2CTenant = envB2CTenant,
                    B2CClientId = envB2CClientId,
                    B2CPolicy = envB2CPolicy,
                    B2CRedirectUri = envB2CRedirectUri
                }.GetLink();

            ServiceBusMessage<InviteMail> msg = new ServiceBusMessage<InviteMail>(new InviteMail
            {
                Email = reg.EmailAddress,
                Partner = envLogisticsPartnerName,
                OrganisationName = reg.CompanyName,
                FirstName = reg.PersonName,
                InviteType = (int)InviteType.NewShipper,
                Link = link,
                AlreadyRegistered = alreadyRegistered
            });

            await _serviceBusClient.Publish<InviteMail>(msg, envInviteQueueName);
        }
    }
}
