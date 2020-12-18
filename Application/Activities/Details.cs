using Application.Activities.MappingModels;
using Application.Errors;
using AutoMapper;
using Domain.Models;
using MediatR;
using Persistence;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<ActivityDto>
    {
      public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, ActivityDto>
    {
      private readonly DataDbContext _context;
      private readonly IMapper _mapper;

      public Handler(DataDbContext context, IMapper mapper)
      {
        _mapper = mapper;
        _context = context;
      }

      public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Id);
        if (activity == null)
          throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });

        var activityReturn = _mapper.Map<Activity, ActivityDto>(activity);

        return activityReturn;
      }
    }
  }
}