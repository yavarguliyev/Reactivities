using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class AddPhoto
  {
    public class CommandPhotoAdd : IRequest<Photo>
    {
      public IFormFile file { get; set; }
    }

    public class Handler : IRequestHandler<CommandPhotoAdd, Photo>
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

      public async Task<Photo> Handle(CommandPhotoAdd request, CancellationToken cancellationToken)
      {
        var photoUploadResult = _photoAccessor.AddPhoto(request.file);

        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUserName());

        var photo = new Photo
        {
          Url = photoUploadResult.Url,
          Id = photoUploadResult.PublicId
        };

        if (!user.Photos.Any(x => x.IsMain))
          photo.IsMain = true;

        user.Photos.Add(photo);

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return photo;

        throw new Exception("Problem saving changes");
      }
    }
  }
}