using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comments.MappingModels;
using Application.Errors;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
  public class Create
  {
    public class CommandCreateComment : IRequest<CommentDto>
    {
      // public CommandCreateComment(string body, Guid activityId, string username)
      // {
      //   this.Body = body;
      //   this.ActivityId = activityId;
      //   this.Username = username;
      // }

      public string Body { get; set; }
      public Guid ActivityId { get; set; }
      public string Username { get; set; }
    }

    public class Handler : IRequestHandler<CommandCreateComment, CommentDto>
    {
      private readonly DataDbContext _context;
      private readonly IMapper _mapper;
      public Handler(DataDbContext context, IMapper mapper)
      {
        _mapper = mapper;
        _context = context;
      }

      public async Task<CommentDto> Handle(CommandCreateComment request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.ActivityId);

        if (activity == null)
          throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

        var comment = new Comment
        {
          Author = user,
          Activity = activity,
          Body = request.Body,
          CreatedAt = DateTime.Now
        };

        activity.Comments.Add(comment);

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return _mapper.Map<CommentDto>(comment);

        throw new Exception("Problem saving changes");
      }
    }
  }
}