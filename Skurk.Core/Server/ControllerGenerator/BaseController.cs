using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Skurk.Core.Server.ControllerGenerator
{
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult<T>> Send()
        {
            return _storage.GetAll();
        }
    }
}
