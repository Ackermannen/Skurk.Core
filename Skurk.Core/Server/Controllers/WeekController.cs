using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Week;
using Skurk.Core.Shared.Week.Queries;

namespace Skurk.Core.Server.Controllers
{
    [ApiController]
    [Route(GenericConstants.ApiRoutePrefix)]
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
        public async Task<ActionResult<RequestResult<WeekDto>>> GetWeek([FromQuery] GetWeekQuery q)
        {
            var res = await _m.Send(q);
            return StatusCode((int)res.HttpStatusCode, res);
        } 
    }
}
