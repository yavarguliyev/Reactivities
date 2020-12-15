using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers.v1
{
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v1/[controller]")]
  public class BaseApiController : ControllerBase
  {
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
  }
}