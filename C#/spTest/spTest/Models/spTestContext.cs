using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace spTest.Models
{
    public partial class spTestContext : DbContext
    {
        public spTestContext()
        {
        }

        public spTestContext(DbContextOptions<spTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=H-HICEIT6-20\\NEWSERVER;Database=spTest;MultipleActiveResultSets=true;User ID=sa;Password=1qaz@WSX");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("Assignment");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Assignmen__Emplo__412EB0B6");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Assignmen__TaskI__4222D4EF");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EmployeeStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmployeeType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeaveEndDate).HasColumnType("date");

                entity.Property(e => e.LeaveStartDate).HasColumnType("date");

                entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Depart__3E52440B");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("Task");

                entity.Property(e => e.Priority)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaskName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TaskStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Task__ProjectId__3B75D760");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
