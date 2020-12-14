using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
    public class Command : IRequest
    {
      public Activity Activity { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataDbContext _context;
      public Handler(DataDbContext context)
      {
        _context = context;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        _context.Activities.Add(request.Activity);

        await _context.SaveChangesAsync();

        return Unit.Value;
      }
    }
  }
}