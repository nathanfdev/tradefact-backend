using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Utilities.ActivationDataSetup.Services
{
    class ActivationDataSetupService : IActivationDataSetupService
    {
        private readonly ILogger<ActivationDataSetupService> _logger;
        private readonly TradefactDbContext _context;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly Guid _tradefactLogisticsOrgId;
        private readonly Guid _tradefactLogisticsUserId;
        private Guid? _orgIdToProcess = null;

        public ActivationDataSetupService(ILoggerFactory loggerFactory, TradefactDbContext context, IServiceBusClient serviceBusClient)
        {
            _logger = loggerFactory.CreateLogger<ActivationDataSetupService>();
            _context = context;
            _serviceBusClient = serviceBusClient;

            var tflUser = _context.Users.AsNoTracking().FirstOrDefault(q => q.Email == "logistics@tradefact.com");
            if (tflUser is null)
            {
                throw new ArgumentException();
            }
            else
            {
                _tradefactLogisticsOrgId = (Guid)tflUser.OrganisationId;
                _tradefactLogisticsUserId = new Guid(tflUser.Id);
            }
        }

        // Allows single organisations to be processed for testing purposes
        public void SetFilter(string email)
        {
            var org = _context.Organisations.AsNoTracking().FirstOrDefault(q => q.ContactEmail == email);
            if (org is null)
            {
                throw new Exception("Specified organisation not found");
            } 
            else
            {
                _orgIdToProcess = org.Id;
            }
        }

        public void FixInvitations(OrganisationTypeEnum orgType, InviteType invType)
        {
            var orgList = _context.Organisations.Where(q => 
                q.OrganisationTypeId == orgType && 
                q.Id != _tradefactLogisticsOrgId &&
                q.ContactEmail != null &&
                q.ParentId == null &&
                (_orgIdToProcess == null || q.Id == _orgIdToProcess)
            ).ToList();

            foreach (var organisation in orgList)
            {
                var adminUser = _context.Users.AsNoTracking().FirstOrDefault(q => q.Email == organisation.ContactEmail);

                fixOrganisationInvitation(invType, organisation, adminUser);

                var userList = _context.Users.Where(q => 
                    q.OrganisationId == organisation.Id && 
                    q.Email != organisation.ContactEmail
                ).ToList();

                foreach (var employee in userList)
                {
                    fixEmployeeInvitation(organisation, adminUser, employee);
                }
            }
        }

        private void fixOrganisationInvitation(InviteType invType, Organisation organisation, ApplicationUser adminUser)
        {
            var invitation = _context.InvitationLog.FirstOrDefault(q => q.EmailAddress == organisation.ContactEmail);

            if (invitation is null)
            {
                _logger.LogInformation("Creating {0} invitation for {1} ({2})", 
                    invType.ToString(), organisation.Name, organisation.ContactEmail);

                invitation = new InvitationLog
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    UserId = adminUser?.Id,
                    InviteType = (int)invType,
                    CompanyName = organisation.Name,
                    GivenName = organisation.ContactName,
                    EmailAddress = organisation.ContactEmail,
                    InviteRequestedByOrganisationId = _tradefactLogisticsOrgId,
                    InviteRequestedByUserId = _tradefactLogisticsUserId,
                    MemberOfOrganisationId = organisation.Id,
                    LastActivationAttempt = organisation.CreationDateInternal,
                    ActivationStatus = adminUser is null ? AccountActivationStatus.Error : AccountActivationStatus.Complete
                };

                _context.InvitationLog.Add(invitation);
            }
            else
            {
                fixExistingInvitation(invitation, organisation, organisation.CreationDateInternal);
            }
        }

        private void fixEmployeeInvitation(Organisation organisation, ApplicationUser adminUser, ApplicationUser employee)
        {
            var invitation = _context.InvitationLog.FirstOrDefault(q => q.EmailAddress == employee.Email);

            if (invitation is null)
            {
                _logger.LogInformation("Creating {0} invitation for {0} ({1})",
                    InviteType.NewEmployee.ToString(), employee.GivenName, employee.Email);

                invitation = new InvitationLog
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    UserId = employee.Id,
                    InviteType = (int)InviteType.NewEmployee,
                    CompanyName = organisation.Name,
                    GivenName = employee.GivenName,
                    EmailAddress = employee.Email,
                    InviteRequestedByOrganisationId = organisation.Id,
                    InviteRequestedByUserId = new Guid(adminUser?.Id),
                    MemberOfOrganisationId = organisation.Id,
                    LastActivationAttempt = employee.RegistrationDate,
                    ActivationStatus = AccountActivationStatus.Complete
                };

                _context.InvitationLog.Add(invitation);
            }
            else
            {
                fixExistingInvitation(invitation, organisation, employee.RegistrationDate);
            }
        }

        // Where and invitation already exists, make sure that all data is correctly set
        private void fixExistingInvitation(InvitationLog invitation, Organisation organisation, DateTime? lastActivationAttempt)
        {
            _logger.LogInformation("Fixing existing {0} invitation for {1} ({2})", 
                ((InviteType)invitation.InviteType).ToString(), invitation.GivenName, invitation.EmailAddress);

            if (invitation.MemberOfOrganisationId is null)
            {
                invitation.MemberOfOrganisationId = organisation.Id;
            }

            if (invitation.LastActivationAttempt is null)
            {
                invitation.LastActivationAttempt = lastActivationAttempt;
            }
        }

        // Each shipper should have a partnerhip with Tradefact Logistics
        public void SetUpTradefactLogistics()
        {
            var shipperList = _context.Organisations.Where(q => 
                q.OrganisationTypeId == OrganisationTypeEnum.SHIPPER &&
                (_orgIdToProcess == null || q.Id == _orgIdToProcess)).ToList();

            foreach (var shipper in shipperList)
            {
                if (!_context.Partnerships.Any(q => 
                    q.ClientId == shipper.Id && 
                    q.ProviderId == _tradefactLogisticsOrgId &&
                    q.PartnershipTypeId == PartnershipTypeEnum.LOGISTICS))
                {
                    _logger.LogInformation("Setting up Tradefact Logistics partnership for {0} ({1})",
                        shipper.Name, shipper.ContactEmail);

                    var partnership = new Partnership
                    {
                        Id = new Guid(),
                        IsActive = true,
                        ClientId = shipper.Id,
                        ProviderId = _tradefactLogisticsOrgId,
                        PartnershipTypeId = PartnershipTypeEnum.LOGISTICS
                    };

                    _context.Partnerships.Add(partnership);
                }
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        // Run activations to ensure that Chargebee subscriptions get set up
        public void QueueActivations()
        {
            var invitationList = _context.InvitationLog.Where(q => 
                q.LastActivationAttempt != null &&
                (_orgIdToProcess == null || q.MemberOfOrganisationId == _orgIdToProcess)).ToList();

            foreach (var invitation in invitationList)
            {
                _logger.LogInformation("Queueing activation for {0}", invitation.EmailAddress);
                var msg = new ServiceBusMessage<InvitationLog>(invitation);
                _serviceBusClient.Publish<InvitationLog>(msg, "account-activate").Wait();
            }
        }
    }

    public interface IActivationDataSetupService
    {
        void SetFilter(string emailPattern);
        void FixInvitations(OrganisationTypeEnum organisationType, InviteType inviteType);
        void SetUpTradefactLogistics();
        void SaveChanges();
        void QueueActivations();
    }
}
