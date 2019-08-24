using Microsoft.EntityFrameworkCore;

namespace SampleExam.Infrastructure
{
    public class SampleExamContext : DbContext
    {

        public SampleExamContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Domain.Value> Values { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseIdentityColumns();
            modelBuilder.Entity<Domain.Value>().HasData(new Domain.Value() { Id = 1, Text = "Sample value 1" });
        }

    }
}