using eLearning.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course_Student>().HasKey(cs => new
            {
                cs.CourseId,
                cs.UserId,
            });
            modelBuilder.Entity<ExerciseResult>().HasKey(tr => new
            {
                tr.ExerciseId,
                tr.UserId,
            });

            modelBuilder.Entity<Exercise>().Property(n => n.Visible).HasDefaultValue(true);
            modelBuilder.Entity<File>().Property(n => n.Visible).HasDefaultValue(true);
            modelBuilder.Entity<Note>().Property(n => n.Visible).HasDefaultValue(true);
            modelBuilder.Entity<Topic>().Property(n => n.Visible).HasDefaultValue(true);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Course_Student> Courses_Students { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseResult> ExerciseResults { get; set; }

    }
}
