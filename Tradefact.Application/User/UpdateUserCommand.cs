using Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.Common.Exceptions;
using Tradefact.Data;

namespace Tradefact.Application.Shipments.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid? Id { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

    }

    public class UpdateUserCommandHandler: IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly TradefactDbContext _context;

        public UpdateUserCommandHandler(TradefactDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Handler which processes the command when
        /// customer executes cancel order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {

            return true;
        }
    }


}
