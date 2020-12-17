using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Activities;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.v1
{
  public class ActivitiesController : BaseApiController
  {
    #region activities
    [Route("list")]
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> List()
    {
      return await Mediator.Send(new List.Query());
    }

    [Route("details/{id}")]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<Activity>> Details(Guid id)
    {
      return await Mediator.Send(new Details.Query { Id = id });
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<Unit>> Create(Create.Command command)
    {
      return await Mediator.Send(command);
    }

    [Route("edit/{id}")]
    [HttpPut]
    public async Task<ActionResult<Unit>> Edit(Guid id, Edit.CommandEdit command)
    {
      command.Id = id;
      return await Mediator.Send(command);
    }

    [Route("delete/{id}")]
    [HttpDelete]
    public async Task<ActionResult<Unit>> DeleteActivity(Guid id)
    {
      return await Mediator.Send(new Delete.Command { Id = id });
    }
    #endregion
  }
}