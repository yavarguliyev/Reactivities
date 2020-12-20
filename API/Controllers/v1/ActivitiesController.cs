using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Activities;
using System;
using Microsoft.AspNetCore.Authorization;
using Application.Activities.MappingModels;

namespace API.Controllers.v1
{
  public class ActivitiesController : BaseApiController
  {
    #region activities
    [HttpGet("list")]
    public async Task<ActionResult<List.ActivitiesEnvelope>> List(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
    {
      return await Mediator.Send(new List.Query(limit, offset, isGoing, isHost, startDate));
    }

    [HttpGet("details/{id}")]
    public async Task<ActionResult<ActivityDto>> Details(Guid id)
    {
      return await Mediator.Send(new Details.Query { Id = id });
    }

    [HttpPost("create")]
    public async Task<ActionResult<Unit>> Create(Create.Command command)
    {
      return await Mediator.Send(command);
    }


    [HttpPut("edit/{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActionResult<Unit>> Edit(Guid id, Edit.CommandEdit command)
    {
      command.Id = id;
      return await Mediator.Send(command);
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActionResult<Unit>> DeleteActivity(Guid id)
    {
      return await Mediator.Send(new Delete.Command { Id = id });
    }

    [HttpPost("attend/{id}/attend")]
    public async Task<ActionResult<Unit>> Attend(Guid id)
    {
      return await Mediator.Send(new Attend.CommandAttend { Id = id });
    }

    [HttpDelete("unattend/{id}/unattend")]
    public async Task<ActionResult<Unit>> UnAttend(Guid id)
    {
      return await Mediator.Send(new UnAttend.CommandDelete { Id = id });
    }
    #endregion
  }
}