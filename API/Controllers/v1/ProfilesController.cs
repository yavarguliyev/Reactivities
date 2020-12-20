using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
  public class ProfilesController : BaseApiController
  {
    #region profiles

    [HttpGet("details/{username}")]
    public async Task<ActionResult<Profile>> Details(string username)
    {
      return await Mediator.Send(new Details.Query { Username = username });
    }

    [HttpPut("edit")]
    public async Task<ActionResult<Unit>> Edit(EditProfile.CommandProfileEdit command)
    {
      return await Mediator.Send(command);
    }

    [HttpGet("getuseractivities/{username}/activities")]
    public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string username, string predicate)
    {
      return await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate });
    }
    #endregion
  }
}