using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
  public class UserController : BaseApiController
  {
    #region user
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(Login.Query query)
    {
      return await Mediator.Send(query);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(Register.RegisterCommand command)
    {
      return await Mediator.Send(command);
    }

    [HttpGet("current-user")]
    public async Task<ActionResult<User>> CurrentUser()
    {
      return await Mediator.Send(new CurrentUser.Query());
    }
    #endregion
  }
}