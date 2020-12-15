using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Edit
  {
    public class CommandEdit : IRequest
    {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public string Category { get; set; }
      public DateTime? Date { get; set; }
      public string City { get; set; }
      public string Venue { get; set; }
    }

    public class Handler : IRequestHandler<CommandEdit>
    {
      private readonly DataDbContext _context;
      public Handler(DataDbContext context)
      {
        _context = context;
      }

      public async Task<Unit> Handle(CommandEdit request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Id);

        if (activity == null)
          throw new Exception("Could not find any activity");

        activity.Title = request.Title ?? activity.Title;
        activity.Description = request.Description ?? activity.Description;
        activity.Category = request.Category ?? activity.Category;
        activity.Date = request.Date ?? activity.Date;
        activity.City = request.City ?? activity.City;
        activity.Venue = request.Venue ?? activity.Venue;

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Unit.Value;

        throw new Exception("Problem saving changes");
      }
    }
  }
}