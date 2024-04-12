using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace StudioManager.Model;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usergame> Usergames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json")
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .Build();

            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("sex", new[] { "М", "Ж" })
            .HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.Property(e => e.IdDepartment).HasColumnName("id_department");
            entity.Property(e => e.Departmentname)
                .HasMaxLength(64)
                .HasDefaultValueSql("'DepartmentName'::character varying")
                .HasColumnName("departmentname");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.IdGame).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Gamedescription).HasColumnName("gamedescription");
            entity.Property(e => e.Gamegenre)
                .HasMaxLength(64)
                .HasDefaultValueSql("'GameGenre'::character varying")
                .HasColumnName("gamegenre");
            entity.Property(e => e.Gamename)
                .HasMaxLength(64)
                .HasDefaultValueSql("'GameName'::character varying")
                .HasColumnName("gamename");
            entity.Property(e => e.Gamereleasedate).HasColumnName("gamereleasedate");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.IdEmployee).HasName("staff_pkey");

            entity.ToTable("staff");

            entity.Property(e => e.IdEmployee).HasColumnName("id_employee");
            entity.Property(e => e.Employeeemail)
                .HasDefaultValueSql("'example@example.com'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("employeeemail");
            entity.Property(e => e.Employeefullname)
                .HasMaxLength(64)
                .HasDefaultValueSql("'FullName'::character varying")
                .HasColumnName("employeefullname");
            entity.Property(e => e.Employeephonenumber)
                .HasDefaultValueSql("'+1234567890'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("employeephonenumber");
            entity.Property(e => e.Employeephoto).HasColumnName("employeephoto");
            entity.Property(e => e.Employeeposition)
                .HasMaxLength(64)
                .HasDefaultValueSql("'Employee'::character varying")
                .HasColumnName("employeeposition");
            entity.Property(e => e.Employeesex)
                .HasConversion<string>()
                .HasColumnType("sex")
                .HasColumnName("employeesex");
            entity.Property(e => e.IdDepartment).HasColumnName("id_department");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Staff)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("id_department_fk");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.IdTask).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdDepartment).HasColumnName("id_department");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Taskgroup)
                .HasMaxLength(64)
                .HasDefaultValueSql("'TaskGroup'::character varying")
                .HasColumnName("taskgroup");
            entity.Property(e => e.Taskisactive).HasColumnName("taskisactive");
            entity.Property(e => e.Taskname)
                .HasMaxLength(64)
                .HasDefaultValueSql("'TaskName'::character varying")
                .HasColumnName("taskname");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("id_department_fk");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdGame)
                .HasConstraintName("id_game_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Useremail)
                .HasMaxLength(64)
                .HasDefaultValueSql("'example@example.com'::character varying")
                .HasColumnName("useremail");
            entity.Property(e => e.Userlogin)
                .HasMaxLength(64)
                .HasDefaultValueSql("'Login'::character varying")
                .HasColumnName("userlogin");
            entity.Property(e => e.Userpassword)
                .HasMaxLength(64)
                .HasColumnName("userpassword");
            entity.Property(e => e.Userprofilepicture).HasColumnName("userprofilepicture");
        });

        modelBuilder.Entity<Usergame>(entity =>
        {
            entity.HasKey(e => e.IdUsergames).HasName("usergames_pkey");

            entity.ToTable("usergames");

            entity.Property(e => e.IdUsergames).HasColumnName("id_usergames");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Usergames)
                .HasForeignKey(d => d.IdGame)
                .HasConstraintName("id_game_fk");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Usergames)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("id_user_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
