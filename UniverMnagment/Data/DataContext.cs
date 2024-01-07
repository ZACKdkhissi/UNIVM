using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UniverMnagment.Entities;

namespace UniverMnagment.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<CourseProfessor> CourseProfessors { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la relation many-to-many entre Course et Professor
            modelBuilder.Entity<CourseProfessor>()
                .HasKey(cp => new { cp.CourseId, cp.ProfessorId });

            modelBuilder.Entity<CourseProfessor>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.CourseProfessors)
                .HasForeignKey(cp => cp.CourseId);

            modelBuilder.Entity<CourseProfessor>()
                .HasOne(cp => cp.Professor)
                .WithMany(p => p.CourseProfessors)
                .HasForeignKey(cp => cp.ProfessorId);

            // Configuration de la relation many-to-many entre Student et Course
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);
        }
    }
}
