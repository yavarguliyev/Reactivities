using System.Net;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;

namespace Application.Photos
{
  public class DeletePhoto
  {
    public class CommandPhotoDelete : IRequest
    {
      public string Id { get; set; }
    }

    public class Handler : IRequestHandler<CommandPhotoDelete>
    {
      private readonly DataDbContext _context;
      private readonly IUserAccessor _userAccessor;
      private readonly IPhotoAccessor _photoAccessor;

      public Handler(DataDbContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
      {
        _photoAccessor = photoAccessor;
        _userAccessor = userAccessor;
        _context = context;
      }

      public async Task<Unit> Handle(CommandPhotoDelete request, CancellationToken cancellationToken)
      {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUserName());

        var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
        if (photo == null)
          throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });

        if (photo.IsMain)
          throw new RestException(HttpStatusCode.BadRequest, new { Photo = "You cannot delete your main photo" });

        var result = _photoAccessor.DeletePhoto(photo.Id);
        if (result == null)
          throw new Exception("Problem deleting the photo");

        user.Photos.Remove(photo);

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Unit.Value;

        throw new Exception("Problem saving changes");
      }
    }
  }
}