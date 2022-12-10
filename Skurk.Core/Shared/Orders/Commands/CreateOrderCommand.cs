using MediatR;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Interfaces;
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

    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, RequestResult<Guid>>
    {
        public Task<RequestResult<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
