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
    [Route("login")]
    [HttpPost]
    public async Task<ActionResult<User>> Login(Login.Query query)
    {
      return await Mediator.Send(query);
    }

    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    public async Task<ActionResult<User>> Register(Register.RegisterCommand command)
    {
      return await Mediator.Send(command);
    }

    [Route("current-user")]
    [HttpGet]
    public async Task<ActionResult<User>> CurrentUser()
    {
      return await Mediator.Send(new CurrentUser.Query());
    }
    #endregion
  }
}