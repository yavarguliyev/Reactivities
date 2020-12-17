using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence
{
  public class DataDbContext : IdentityDbContext<AppUser>
  {
    public DataDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Activity> Activities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}