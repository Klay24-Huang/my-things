using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace For_Interview.Models.DbModels;

public partial class MyDBContext : DbContext
{
    public MyDBContext()
    {
    }

    public MyDBContext(DbContextOptions<MyDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplyFile> ApplyFiles { get; set; }

    public virtual DbSet<Org> Orgs { get; set; }

    public virtual DbSet<Syslog> Syslogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplyFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__apply_fi__3213E83F2A007780");

            entity.ToTable("apply_file");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.FilePath)
                .HasMaxLength(100)
                .HasColumnName("file_path");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ApplyFiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__apply_fil__user___4316F928");
        });

        modelBuilder.Entity<Org>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orgs__3213E83F11238F2D");

            entity.ToTable("orgs");

            entity.HasIndex(e => e.Title, "UQ__orgs__E52A1BB3812D8994").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<Syslog>(entity =>
        {
            entity.HasKey(e => e.SeqNo).HasName("PK__syslog__4B660EB1E4C704EC");

            entity.ToTable("syslog");

            entity.Property(e => e.SeqNo).HasColumnName("seq_no");
            entity.Property(e => e.Account)
                .HasMaxLength(15)
                .HasColumnName("account");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .HasColumnName("ipaddress");
            entity.Property(e => e.LoginAt)
                .HasColumnType("datetime")
                .HasColumnName("login_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F6BD77EE2");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164B9BE1F72").IsUnique();

            entity.HasIndex(e => e.Account, "UQ__users__EA162E1160DC5525").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Account)
                .HasMaxLength(15)
                .HasColumnName("account");
            entity.Property(e => e.Birthday)
                .HasColumnType("datetime")
                .HasColumnName("birthday");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .HasColumnName("name");
            entity.Property(e => e.OrgId).HasColumnName("org_id");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Org).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__org_id__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
