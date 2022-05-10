using ChargeBee.Api;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Data;
using Tradefact.UserManagement.Model;

namespace Tradefact.UserManagement.Functions
{
    public class AccountFunctions
    {
        private static readonly string qActivateAccount = Environment.GetEnvironmentVariable("Queue_AccountActivate", EnvironmentVariableTarget.Process);
        private static readonly string TradefactDBConnectionString = Environment.GetEnvironmentVariable("TradefactDB", EnvironmentVariableTarget.Process);
        private static readonly string envChargebeeSiteName = Environment.GetEnvironmentVariable("ChargebeeSiteName", EnvironmentVariableTarget.Process);
        private static readonly string envChargebeeApiKey = Environment.GetEnvironmentVariable("ChargebeeApiKey", EnvironmentVariableTarget.Process);
        private static readonly string envChargebeeDefaultPlanId = Environment.GetEnvironmentVariable("ChargebeeDefaultPlanId", EnvironmentVariableTarget.Process);
        private static readonly string envChargebeePartnerPlanId = Environment.GetEnvironmentVariable("ChargebeePartnerPlanId", EnvironmentVariableTarget.Process);
        private static readonly Guid envTradefactLogisticsOrgId = new(Environment.GetEnvironmentVariable("TradefactLogisticsOrgId", EnvironmentVariableTarget.Process));

        protected readonly TradefactDbContext _context;
        protected readonly ITradefactActivityService _tradefactActivityService;
        protected readonly IBillingService _billingService;

        public AccountFunctions(TradefactDbContext context, IBillingService billingService, ITradefactActivityService tradefactActivityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tradefactActivityService = tradefactActivityService ?? throw new ArgumentNullException(nameof(tradefactActivityService));

            // Configure ChargeBee API
            ApiConfig.Configure(envChargebeeSiteName, envChargebeeApiKey);
            _billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
        }

        [FunctionName(nameof(ActivateAccountOrchestrator))]
        public async Task<ActivateAccountResult> ActivateAccountOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Core.ServiceBus.ServiceBusMessage<InvitationLog> payload = context.GetInput<Core.ServiceBus.ServiceBusMessage<InvitationLog>>();

            var retryOptions = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(3), maxNumberOfAttempts: 5)
            {
                BackoffCoefficient = 1.5
            };

            ActivateAccountResult progress = new ActivateAccountResult
            {
                Invitation = payload.Body,
                BillingSetupComplete = false
            };

            if (progress.Invitation != null)
            {
                // Get/Create Org - return org id
                progress = await context.CallActivityWithRetryAsync<ActivateAccountResult>(
                    nameof(ActivateOrganisation), retryOptions, progress);

                // Get / Create User account (entry in aspnet users table)
                progress = await context.CallActivityWithRetryAsync<ActivateAccountResult>(
                    nameof(ActivateUser), retryOptions, progress);

                // Do chargebee integrations
                progress = await context.CallActivityWithRetryAsync<ActivateAccountResult>(
                    nameof(ActivateChargebeeSubscription), retryOptions, progress);

                // Update status of pending activation
                progress = await context.CallActivityWithRetryAsync<ActivateAccountResult>(
                    nameof(UpdateActivationStatus), retryOptions, progress);
            }

            return progress;
        }

        // Get or Create Organisation associated with this account
        [FunctionName(nameof(ActivateOrganisation))]
        public async Task<ActivateAccountResult> ActivateOrganisation([ActivityTrigger] ActivateAccountResult progress, ILogger log)
        {
            InvitationLog invite = await _context.InvitationLog.FirstOrDefaultAsync(q => q.Id == progress.Invitation.Id);
            if (invite == null)
            {
                return progress;
            }

            Organisation org = await this.GetOrCreateOrganisation(invite, log);
            if (org == null)
            {
                return progress;
            }

            if (org.Id == null || org.Id == Guid.Empty) // Meaning a new organisation was created
            {
                org.Id = Guid.NewGuid();
                _context.Organisations.Add(org);

                invite.MemberOfOrganisationId = org.Id; // Record in case activation fails and resumes later

                var invitingOrg = await _context.Organisations.AsNoTracking().FirstOrDefaultAsync(q => q.Id == invite.InviteRequestedByOrganisationId);

                // SHIPPER invited by PARTNER
                if (org.OrganisationTypeId == OrganisationTypeEnum.SHIPPER &&
                    invitingOrg.OrganisationTypeId == OrganisationTypeEnum.PARTNER)
                {
                    addPartner(envTradefactLogisticsOrgId, org.Id, Core.Enums.PartnershipTypeEnum.LOGISTICS);

                    if (invitingOrg.Id != envTradefactLogisticsOrgId) // Don't add Tradefact Logistics twice
                    {
                        addPartner(invitingOrg.Id, org.Id, Core.Enums.PartnershipTypeEnum.LOGISTICS);
                    }
                }

                // SHIPPER invited by SHIPPER
                else if (org.OrganisationTypeId == OrganisationTypeEnum.SHIPPER &&
                    invitingOrg.OrganisationTypeId == OrganisationTypeEnum.SHIPPER)
                {
                    addPartner(envTradefactLogisticsOrgId, org.Id, Core.Enums.PartnershipTypeEnum.LOGISTICS);
                    addB2BConnection(org.Id, invitingOrg.Id);
                    addB2BConnection(invitingOrg.Id, org.Id);
                }

                // PARTNER invited by SHIPPER
                else if (org.OrganisationTypeId == OrganisationTypeEnum.PARTNER &&
                    invitingOrg.OrganisationTypeId == OrganisationTypeEnum.SHIPPER &&
                    invite.InviteType == (int)InviteType.NewFreightForwarderInvitedByShipper)
                {
                    addPartner(org.Id, invitingOrg.Id, Core.Enums.PartnershipTypeEnum.LOGISTICS);
                }
            }

            _ = await _context.SaveChangesAsync();

            progress.Invitation = invite;
            progress.Organisation = org;

            return progress;

            void addPartner(Guid providerId, Guid clientId, Core.Enums.PartnershipTypeEnum partnershipType)
            {
                var newPartnership = new Partnership
                {
                    ProviderId = providerId,
                    ClientId = clientId,
                    PartnershipTypeId = partnershipType
                };
                _context.Partnerships.Add(newPartnership);
            }

            void addB2BConnection(Guid organisationId, Guid linkedOrganisationId)
            {
                var newB2BConnection = new B2BConnection
                {
                    OrgansationId = organisationId,
                    LinkedOrganisationId = linkedOrganisationId,
                    ConnectionStatus = ConnectionStatusEnum.Active
                };
                _context.B2BConnections.Add(newB2BConnection);
            }
        }

        [FunctionName(nameof(ActivateUser))]
        public async Task<ActivateAccountResult> ActivateUser([ActivityTrigger] ActivateAccountResult progress, ILogger log)
        {
            var invite = progress.Invitation;
            var org = progress.Organisation;

            if (org != null)
            {
                progress.ActiveUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(q => q.Email == progress.Invitation.EmailAddress);
                if (progress.ActiveUser == null)
                {
                    Guid assignToOrgId = invite.MemberOfOrganisationId.HasValue ? invite.MemberOfOrganisationId.GetValueOrDefault() : org.Id;
                    ApplicationUser new_user = this.GenerateUser(invite, assignToOrgId);
                    _context.Users.Add(new_user);
                    _ = await _context.SaveChangesAsync();

                    progress.ActiveUser = new_user;

                    if ((InviteType)progress.Invitation.InviteType != InviteType.NewEmployee )
                    {
                        var invitingOrg = await _context.Organisations.AsNoTracking().FirstOrDefaultAsync(q => q.Id == invite.InviteRequestedByOrganisationId);

                        if (invitingOrg.ContactEmail == "logistics@tradefact.com" && !string.IsNullOrEmpty(invite.SubscriptionPlan))
                        {
                            await _tradefactActivityService.TrackAnonymousEvent(
                                invite.Id,
                                invite.GivenName,
                                invite.EmailAddress, 
                                invite.CompanyName, 
                                "invitation_accepted", 
                                new TrackWith { Segment = true, Tradefact = true },
                                new EventProps { Segment = invite }
                            );
                        }

                        await _tradefactActivityService.TrackEvent(
                            new_user.Id,
                            new_user.FullName, 
                            new_user.Email, 
                            org, 
                            invitingOrg,
                            "account_created",
                            new TrackWith { Segment = true, Tradefact = true },
                            new EventProps { Segment = invite, Tradefact = new TradefactEventProps { Reference = invite.EmailAddress, Type = ActivityTypeEnum.INVITE, Entity = ActivityEntityTypeEnum.NETWORK, CustomDescription = "Invite Accepted" } }
                        );
                    }
                }
            }

            return progress;
        }

        private string selectPlan(Organisation org, InvitationLog invite)
        {
            return org.OrganisationTypeId switch
            {
                var t when t == 
                    OrganisationTypeEnum.SHIPPER => invite.SubscriptionPlan ?? envChargebeeDefaultPlanId,
                    OrganisationTypeEnum.PARTNER => envChargebeePartnerPlanId,
                    _ => null
            };
        }

        [FunctionName(nameof(ActivateChargebeeSubscription))]
        public async Task<ActivateAccountResult> ActivateChargebeeSubscription([ActivityTrigger] ActivateAccountResult progress, ILogger log)
        {
            var invite = progress.Invitation;
            var org = await _context.Organisations.FirstOrDefaultAsync(q => q.Id == progress.Organisation.Id);
            var user = progress.ActiveUser;
            progress.BillingSetupComplete = false;

            if (org != null && user != null)
            {
                if (org.ChargebeeSubscriptionId == null)
                {
                    var planId = selectPlan(org, invite);
                    if (!string.IsNullOrEmpty(planId))
                    {
                        // Create Chargebee subscription
                        org.ChargebeeSubscriptionId = await _billingService.CreateSubscription(planId, org.Id.ToString(), org.Name, invite.EmailAddress);
                        _ = await _context.SaveChangesAsync();
                    }
                }
                else if (org.OrganisationTypeId != OrganisationTypeEnum.PARTNER)
                {
                    // Update the number of additional users on the subscription
                    int numAddUsers = await _context.Users.CountAsync(q => q.OrganisationId == org.Id && q.Status != UserStatus.Disabled) - 1;

                    string subscriptionComment = $"Added {user.Email}";
                    if (invite.InviteRequestedByUserId != null)
                    {
                        ApplicationUser invitedBy = await _context.Users.FirstOrDefaultAsync(q => q.Id == invite.InviteRequestedByUserId.ToString());
                        
                        if (invitedBy != null)
                        {
                            subscriptionComment += $", invited by {invitedBy.FullName}";
                        }
                    }

                    _ = await _billingService.UpdateSubscriptionUsers(org.ChargebeeSubscriptionId, numAddUsers, subscriptionComment);
                }

                progress.Organisation = org;
                progress.BillingSetupComplete = true;
            }

            return progress;
        }

        [FunctionName(nameof(UpdateActivationStatus))]
        public async Task<ActivateAccountResult> UpdateActivationStatus([ActivityTrigger] ActivateAccountResult progress, ILogger log)
        {
            progress.Invitation = await _context.InvitationLog.FirstOrDefaultAsync(q => q.Id == progress.Invitation.Id);

            // Update status
            if (progress.Organisation is null || progress.ActiveUser is null || !progress.BillingSetupComplete)
            {
                progress.Invitation.ActivationStatus = AccountActivationStatus.Error;
            }
            else
            {
                progress.Invitation.ActivationStatus = AccountActivationStatus.Complete;
            }

            _ = await _context.SaveChangesAsync();

            return progress;
        }

        private ApplicationUser GenerateUser(InvitationLog invite, Guid organisationId)
        {
            Guid userid = Guid.NewGuid();

            return new ApplicationUser
            {
                Id = userid.ToString(),
                UserName = invite.EmailAddress,
                Email = invite.EmailAddress,
                NormalizedUserName = invite.EmailAddress.ToUpper(),
                NormalizedEmail = invite.EmailAddress.ToUpper(),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                FullName = invite.GivenName,
                OrganisationId = organisationId,
                InvitationId = invite.Id,
                IsAdmin = (InviteType)invite.InviteType != InviteType.NewEmployee
            };
        }

        private async Task<Organisation> GetOrCreateOrganisation(InvitationLog invite, ILogger log)
        {
            if (invite.MemberOfOrganisationId != null)
            {
                return await _context.Organisations.FirstOrDefaultAsync(q => q.Id == invite.MemberOfOrganisationId);
            }

            InviteType invitationType = (InviteType)invite.InviteType;
            Organisation org = invitationType switch
            {
                var invitetype when
                    invitetype == InviteType.NewFreightForwarderInvitedByShipper ||
                    invitetype == InviteType.NewFreightForwarder ||
                    invitetype == InviteType.NewShipper ||
                    invitetype == InviteType.NewBuyer ||
                    invitetype == InviteType.NewShipperNoPartnership ||
                    invitetype == InviteType.NewSupplier => new Organisation
                    {
                        ContactEmail = invite.EmailAddress,
                        CreatedByUser = invite.CreatedByUser,
                        Name = invite.CompanyName,
                        OrganisationTypeId = invitationType switch
                        {
                            var t when
                                t == InviteType.NewFreightForwarder ||
                                t == InviteType.NewFreightForwarderInvitedByShipper => OrganisationTypeEnum.PARTNER,
                            InviteType.NewShipper => OrganisationTypeEnum.SHIPPER,
                            InviteType.NewBuyer => OrganisationTypeEnum.SHIPPER,
                            InviteType.NewShipperNoPartnership => OrganisationTypeEnum.PARTNER,
                            InviteType.NewSupplier => OrganisationTypeEnum.PARTNER,
                            _ => OrganisationTypeEnum.UNKNOWN
                        }
                    },
                InviteType.NewEmployee => await _context.Organisations.FirstOrDefaultAsync(x => x.Id == invite.MemberOfOrganisationId),
                _ => null
            };
            return org;
        }

        [FunctionName(nameof(Activate))]
        public async Task Activate(
            [DurableClient] IDurableOrchestrationClient starter,
            [ServiceBusTrigger("account-activate", Connection = "ServiceBusConnection")] Microsoft.Azure.ServiceBus.Message message,
            ILogger log)
        {
            string inputMessage = Encoding.UTF8.GetString(message.Body);
            Core.ServiceBus.ServiceBusMessage<InvitationLog> payload = JsonConvert.DeserializeObject<Core.ServiceBus.ServiceBusMessage<InvitationLog>>(inputMessage);

            log.LogInformation($"message - " + inputMessage);

            string instanceId = await starter.StartNewAsync(nameof(ActivateAccountOrchestrator), payload);
            log.LogInformation($"Orchestration Started with ID: {instanceId}");
        }

        [FunctionName(nameof(IsDeployed))]
        public static async Task<HttpResponseMessage> IsDeployed(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req)
        {
            var response = new
            {
                ActivateAccountQueue = qActivateAccount,
                ConnectionString = TradefactDBConnectionString,
                ChargebeeSiteName = envChargebeeSiteName,
                ChargebeeApiKey = envChargebeeApiKey,
                TradefactLogisticsOrgId = envTradefactLogisticsOrgId
            };
            string jsonresp = JsonConvert.SerializeObject(response);

            return new HttpResponseMessage
            {

                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(jsonresp, System.Text.Encoding.UTF8, "application/json")
            };
        }

#if DEBUG
        // For dev testing
        [FunctionName(nameof(TestActivation))]
        public async Task<HttpResponseMessage> TestActivation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var message = new Microsoft.Azure.ServiceBus.Message
            { 
                // stuff in here
            };
            await this.Activate(starter, message, log);

            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
#endif
    }
}