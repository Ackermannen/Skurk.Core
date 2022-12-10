using MediatR;
using Skurk.Core.Shared.Interfaces;
using Skurk.Core.Shared.Mediator;
using Skurk.Core.Shared.Orders.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Orders.Queries
{
    public record GetPaginatedOrdersQuery : Getable<IList<OrderDto>>
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    public class GetPaginatedOrdersQueryHandler : IQueryHandler<GetPaginatedOrdersQuery, QueryResult<IList<OrderDto>>>
    {
        public Task<QueryResult<IList<OrderDto>>> Handle(GetPaginatedOrdersQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
