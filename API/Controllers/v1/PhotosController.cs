using System.Threading.Tasks;
using Application.Photos;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
  public class PhotosController : BaseApiController
  {
    #region photos

    [Route("add")]
    [HttpPost]
    public async Task<ActionResult<Photo>> Add([FromForm] AddPhoto.CommandPhotoAdd command)
    {
      return await Mediator.Send(command);
    }

    [Route("delete/{id}")]
    [HttpDelete]
    public async Task<ActionResult<Unit>> Delete(string id)
    {
      return await Mediator.Send(new DeletePhoto.CommandPhotoDelete { Id = id });
    }

    [Route("setmain/{id}/setmain")]
    [HttpPost]
    public async Task<ActionResult<Unit>> SetMain(string id)
    {
      return await Mediator.Send(new SetMain.CommandSetMainPhoto { Id = id });
    }
    #endregion
  }
}