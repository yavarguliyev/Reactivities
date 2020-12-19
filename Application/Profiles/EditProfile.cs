using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
  public class EditProfile
  {
    public class CommandProfileEdit : IRequest
    {
      public string DisplayName { get; set; }
      public string Bio { get; set; }
    }

    public class CommandValidator : AbstractValidator<CommandProfileEdit>
    {
      public CommandValidator()
      {
        RuleFor(x => x.DisplayName).NotEmpty();
      }
    }

    public class Handler : IRequestHandler<CommandProfileEdit>
    {
      private readonly DataDbContext _context;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataDbContext context, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
      }

      public async Task<Unit> Handle(CommandProfileEdit request, CancellationToken cancellationToken)
      {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUserName());

        user.DisplayName = request.DisplayName ?? user.DisplayName;
        user.Bio = request.Bio ?? user.Bio;

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Unit.Value;

        throw new Exception("Problem saving changes");
      }
    }
  }
}