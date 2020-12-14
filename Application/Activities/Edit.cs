using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Edit
  {
    public class Command : IRequest
    {
      public Activity Activity { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataDbContext _context;
      private readonly IMapper _mapper;
      public Handler(DataDbContext context, IMapper mapper)
      {
        _mapper = mapper;
        _context = context;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Activity.Id);

        _mapper.Map(request.Activity, activity);

        await _context.SaveChangesAsync();

        return Unit.Value;
      }
    }
  }
}