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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=127.0.0.1,1434;Database=for_interview; TrustServerCertificate=True;User ID=sa;Password=r^JJo032+A0^");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplyFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__apply_fi__3213E83FD1B9C12F");

            entity.ToTable("apply_file");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.FilePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("file_path");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ApplyFiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__apply_fil__user___48CFD27E");
        });

        modelBuilder.Entity<Org>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orgs__3213E83F9EF6B069");

            entity.ToTable("orgs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<Syslog>(entity =>
        {
            entity.HasKey(e => e.SeqNo).HasName("PK__syslog__4B660EB12C5C51C1");

            entity.ToTable("syslog");

            entity.Property(e => e.SeqNo).HasColumnName("seq_no");
            entity.Property(e => e.Account)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("account");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ipaddress");
            entity.Property(e => e.LoginAt)
                .HasColumnType("datetime")
                .HasColumnName("login_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F39EC610C");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E616479FB723C").IsUnique();

            entity.HasIndex(e => e.Account, "UQ__users__EA162E1157CCEC7D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Account)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("account");
            entity.Property(e => e.Birthday)
                .HasColumnType("datetime")
                .HasColumnName("birthday");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OrgId).HasColumnName("org_id");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Org).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__org_id__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
