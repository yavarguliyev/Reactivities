using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Persistence
{
  public class DataDbContext : DbContext
  {
    public DataDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Activity> Activities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
  }
}
