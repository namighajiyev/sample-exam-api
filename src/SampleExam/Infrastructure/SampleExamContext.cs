using Microsoft.EntityFrameworkCore;
using SampleExam.Domain;

namespace SampleExam.Infrastructure
{
    public class SampleExamContext : DbContext
    {

        public SampleExamContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseIdentityColumns();
            modelBuilder.Entity<Value>().HasData(new Value() { Id = 1, Text = "Sample value 1" });

            modelBuilder.Entity<User>().Property(e => e.Firstname).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(e => e.Lastname).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(e => e.Middlename).HasMaxLength(200);
            modelBuilder.Entity<User>().Property(e => e.Dob).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.Email).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(e => e.Password).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.IsEmailConfirmed).HasDefaultValue(false);
            modelBuilder.Entity<User>().Property(e => e.CreatedAt).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.UpdatedAt).IsRequired(false);

        }

    }
}