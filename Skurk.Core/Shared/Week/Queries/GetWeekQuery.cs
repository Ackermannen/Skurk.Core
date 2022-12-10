using Skurk.Core.Shared.Interfaces;
using Skurk.Core.Shared.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Week.Queries
{
    public record GetWeekQuery : Getable<WeekDto>
    {
        public DateOnly Date { get; set; }
    }

    public class GetWeekQueryHandler : IQueryHandler<GetWeekQuery, QueryResult<WeekDto>>
    {
        public Task<QueryResult<WeekDto>> Handle(GetWeekQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
