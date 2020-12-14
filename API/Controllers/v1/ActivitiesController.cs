using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Activities;
using System;

namespace API.Controllers.v1
{
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v1/[controller]")]
  public class ActivitiesController : ControllerBase
  {
    #region activities
    private IMediator _mediator;
    public ActivitiesController(IMediator meditor)
    {
      _mediator = meditor;
    }

    [Route("list")]
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> List()
    {
      return await _mediator.Send(new List.Query());
    }

    [Route("details/{id}")]
    [HttpGet]
    public async Task<ActionResult<Activity>> Details(Guid id)
    {
      return await _mediator.Send(new Details.Query { Id = id });
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<Unit>> Create(Activity activity)
    {
      return Ok(await _mediator.Send(new Create.Command { Activity = activity }));
    }

    [Route("edit/{id}")]
    [HttpPut]
    public async Task<ActionResult<Unit>> Edit(Guid id, Activity activity)
    {
      activity.Id = id;
      return Ok(await _mediator.Send(new Edit.Command { Activity = activity }));
    }

    [Route("delete/{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      return Ok(await _mediator.Send(new Delete.Command { Id = id }));
    }
    #endregion
  }
}