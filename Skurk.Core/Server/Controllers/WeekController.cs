﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Mediator;
using Skurk.Core.Shared.Week;
using Skurk.Core.Shared.Week.Queries;

namespace Skurk.Core.Server.Controllers
{
    [ApiController]
    [Route("api/")]
    public class WeekController : ControllerBase
    {
        private readonly ILogger<WeekController> _l;
        private readonly IMediator _m;

        public WeekController(IMediator m, ILogger<WeekController> l)
        {
            _m = m;
            _l = l;
        }

        [HttpGet]
        [Route(WeekRoutes.GetWeekQuery)]
        public async Task<ActionResult<QueryResult<WeekDto>>> GetWeek([FromQuery] GetWeekQuery q) => await _m.Send(q);

        [HttpGet]
        [Route(WeekRoutes.GetWeeksQuery)]
        public async Task<ActionResult<QueryResult<WeekDto>>> GetWeeks([FromQuery] GetWeekQuery q) => await _m.Send(q);
    }
}