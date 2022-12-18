using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Enums;
using Skurk.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var startDay = request.Date;
            var endDay = DateOnly.FromDayNumber(startDay.DayNumber).AddDays(7);
            return Task.FromResult(RequestResult<WeekDto>.Success(new WeekDto
            {
                StartDay = startDay,
                EndDay = endDay,
                TimeTasks = new List<TimeTaskDto>()
                {
                    new TimeTaskDto { Id = Guid.NewGuid(), TaskId= Guid.NewGuid(), Times = new float[] { 0,1,2,3,4,5,6 } }
                }
            }, HttpStatusCode.OK));
        }
    }
}
