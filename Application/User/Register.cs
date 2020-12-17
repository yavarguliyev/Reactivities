using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Validators;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User
{
  public class Register
  {
    public class RegisterCommand : IRequest<User>
    {
      public string DisplayName { get; set; }
      public string UserName { get; set; }
      public string Email { get; set; }
      public string Password { get; set; }
    }

    public class CommandValidator : AbstractValidator<RegisterCommand>
    {
      public CommandValidator()
      {
        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).Password();
      }
    }

    public class Handler : IRequestHandler<RegisterCommand, User>
    {
      private readonly UserManager<AppUser> _userManager;
      private readonly IJwtGenerator _jwtGenerator;
      private readonly DataDbContext _context;

      public Handler(DataDbContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
      {
        _context = context;
        _jwtGenerator = jwtGenerator;
        _userManager = userManager;
      }

      public async Task<User> Handle(RegisterCommand request, CancellationToken cancellationToken)
      {
        if (await _context.Users.Where(x => x.Email == request.Email).AnyAsync())
          throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

        if (await _context.Users.Where(x => x.UserName == request.UserName).AnyAsync())
          throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exists" });

        var user = new AppUser
        {
          DisplayName = request.DisplayName,
          UserName = request.UserName,
          Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
          return new User
          {
            DisplayName = user.DisplayName,
            Token = _jwtGenerator.CreateToken(user),
            UserName = user.UserName,
            Image = null
          };
        }

        throw new Exception("Problem creating user");
      }
    }
  }
}