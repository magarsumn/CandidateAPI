using CandidateAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CandidateAPI.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enforce unique constraint on Email field
            builder.Entity<Candidate>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
        public DbSet<Candidate> Candidates { get; set; }
    }
}