using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
  public class ProfilesController : BaseApiController
  {
    #region profiles

    [Route("details/{username}")]
    [HttpGet]
    public async Task<ActionResult<Profile>> Details(string username)
    {
      return await Mediator.Send(new Details.Query { UserName = username });
    }
    #endregion
  }
}