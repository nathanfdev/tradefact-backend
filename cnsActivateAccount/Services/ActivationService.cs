using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;

namespace cnsActivateAccount.Services
{
    class ActivationService : IActivationService
    {
        private readonly ILogger<ActivationService> _logger;
        private readonly TradefactDbContext _context;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly Guid _tradefactLogisticsOrgId;
        private readonly Guid _tradefactLogisticsUserId;
        private Guid? _orgIdToProcess = null;

        public ActivationService(ILoggerFactory loggerFactory, TradefactDbContext context, IServiceBusClient serviceBusClient)
        {
            _logger = loggerFactory.CreateLogger<ActivationService>();
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

        public async Task QueueActivation(Guid invitationId)
        {
            InvitationLog invitation = await _context.InvitationLog.FirstOrDefaultAsync(q => q.Id == invitationId);

            if (invitation?.ActivationStatus  == AccountActivationStatus.Pending)
            {
                _logger.LogInformation("Queueing activation for {0}", invitation.EmailAddress);

                var msg = new ServiceBusMessage<InvitationLog>(invitation);

                _serviceBusClient.Publish<InvitationLog>(msg, "account-activate").Wait();
            }
        }
    }

    public interface IActivationService
    {
        Task QueueActivation(Guid invitationId);
    }
}
