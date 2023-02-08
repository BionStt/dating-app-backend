using Matching.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Matching.Application.Infrastructure.Persistence
{
    public class MatchingDbContext : DbContext
    {
        public MatchingDbContext(DbContextOptions<MatchingDbContext> options) : base(options) { }
        public DbSet<Swipe> Swipes => Set<Swipe>();
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IApplication).Assembly);
        }
    }
}
