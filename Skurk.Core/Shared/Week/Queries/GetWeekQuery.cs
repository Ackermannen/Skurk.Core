using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Interfaces;
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

    public class GetWeekQueryHandler : IQueryHandler<GetWeekQuery, RequestResult<WeekDto>>
    {
        public Task<RequestResult<WeekDto>> Handle(GetWeekQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(RequestResult<WeekDto>.Success(new WeekDto
            {
                StartDay = DateOnly.FromDateTime(new DateTime(2022, 12, 5)),
                EndDay = DateOnly.FromDateTime(new DateTime(2022, 12, 11)),
                TimeTasks = new List<TimeTaskDto>()
                {
                    new TimeTaskDto { Id = Guid.NewGuid(), TaskId= Guid.NewGuid(), Times = new float[7] }
                }
            }, Enums.SuccessHttpStatusCode.OK));
        }
    }
}
