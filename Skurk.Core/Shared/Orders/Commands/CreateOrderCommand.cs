using MediatR;
using Skurk.Core.Shared.Interfaces;
using Skurk.Core.Shared.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Orders.Commands
{
    public record CreateOrderCommand : Postable<Guid>
    {
        public required string Title { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
    }

    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CommandResult<Guid>>
    {
        public Task<CommandResult<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
