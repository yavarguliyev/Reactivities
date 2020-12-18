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
  public class SetMain
  {
    public class CommandSetMainPhoto : IRequest
    {
      public string Id { get; set; }
    }

    public class Handler : IRequestHandler<CommandSetMainPhoto>
    {
      private readonly DataDbContext _context;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataDbContext context, IUserAccessor userAccessor)
      {
        _context = context;
        _userAccessor = userAccessor;
      }

      public async Task<Unit> Handle(CommandSetMainPhoto request, CancellationToken cancellationToken)
      {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUserName());

        var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

        if (photo == null)
          throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        currentMain.IsMain = false;
        photo.IsMain = true;

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Unit.Value;

        throw new Exception("Problem saving changes");
      }
    }
  }
}