using Microsoft.EntityFrameworkCore;
using Test.Scenario.Entities.Resumes;

namespace Test.Scenario.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Resume> Resumes => Set<Resume>();
        public DbSet<ResumeSearchResult> ResumeSearchResults => Set<ResumeSearchResult>();

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<ResumeSearchResult>().HasNoKey(); // keyless for raw SQL projection
        }
    }
}
