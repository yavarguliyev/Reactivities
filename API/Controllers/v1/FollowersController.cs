using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Followers;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
  [Route("api/v1/profiles")]
  public class FollowersController : BaseApiController
  {
    [Route("follow/{username}/follow")]
    [HttpPost]
    public async Task<ActionResult<Unit>> Follow(string username)
    {
      return await Mediator.Send(new Add.CommandAddFollowers { Username = username });
    }

    [Route("unfollow/{username}/follow")]
    [HttpDelete]
    public async Task<ActionResult<Unit>> Unfollow(string username)
    {
      return await Mediator.Send(new Delete.CommandDeleteFollowers { Username = username });
    }

    [Route("getfollowings/{username}/follow")]
    [HttpGet]
    public async Task<ActionResult<List<Profile>>> GetFollowings(string username, string predicate)
    {
      return await Mediator.Send(new List.Query { Username = username, Predicate = predicate });
    }
  }
}