using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Core;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BaseApiController : ControllerBase
    {

        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        // Handle response based on the result returned.
        protected IActionResult HandleResult<T>(ApiResponse<T> result)
        {
            if(result== null) return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }

    }
}