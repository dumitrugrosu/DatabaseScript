using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DatabaseScript.Models;

public partial class AuxDbContext : DbContext
{
    public AuxDbContext()
    {
    }

    public AuxDbContext(DbContextOptions<AuxDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuxBarge> AuxBarges { get; set; }

    public virtual DbSet<AuxBerth> AuxBerths { get; set; }

    public virtual DbSet<AuxEstimatedTime> AuxEstimatedTimes { get; set; }

    public virtual DbSet<AuxFlag> AuxFlags { get; set; }

    public virtual DbSet<AuxMovement> AuxMovements { get; set; }

    public virtual DbSet<AuxMovementTug> AuxMovementTugs { get; set; }

    public virtual DbSet<AuxPilot> AuxPilots { get; set; }

    public virtual DbSet<AuxSetting> AuxSettings { get; set; }

    public virtual DbSet<AuxTug> AuxTugs { get; set; }

    public virtual DbSet<AuxType> AuxTypes { get; set; }

    public virtual DbSet<AuxUser> AuxUsers { get; set; }

    public virtual DbSet<AuxWarn> AuxWarns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=aux_db;uid=root;pwd=Panatha4ever", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.6-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<AuxBarge>(entity =>
        {
            entity.HasKey(e => e.IdBarge).HasName("PRIMARY");

            entity.ToTable("aux_barge");

            entity.Property(e => e.IdBarge)
                .HasColumnType("int(11)")
                .HasColumnName("id_barge");
            entity.Property(e => e.AdminValidated)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("admin_validated");
            entity.Property(e => e.Barge)
                .HasMaxLength(45)
                .HasColumnName("barge");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<AuxBerth>(entity =>
        {
            entity.HasKey(e => e.IdBerth).HasName("PRIMARY");

            entity.ToTable("aux_berth");

            entity.Property(e => e.IdBerth)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_berth");
            entity.Property(e => e.Berth)
                .HasMaxLength(20)
                .HasColumnName("berth");
            entity.Property(e => e.Poz)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("Ma ajuta sa imi dau seama care dana pe unde este in port, pentru dane cu nume de genul \"Doua Dane\", \"Gabare\", \"Port lucru\". Ia numele celei mai apropiate dane numerice. Ex: Doua Dane ar trebui sa fie -2, Dana Gabare -1, RoRo1 0.25, roro4: 0.75")
                .HasColumnName("poz");
        });

        modelBuilder.Entity<AuxEstimatedTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("aux_estimated_time");

            entity.HasIndex(e => new { e.IdAux, e.A, e.B, e.Unit }, "base").IsUnique();

            entity.Property(e => e.A)
                .HasColumnType("int(11)")
                .HasColumnName("a");
            entity.Property(e => e.B)
                .HasColumnType("int(11)")
                .HasColumnName("b");
            entity.Property(e => e.IdAux)
                .HasColumnType("int(11)")
                .HasColumnName("id_aux");
            entity.Property(e => e.LastStamp)
                .HasColumnType("bigint(20)")
                .HasColumnName("last_stamp");
            entity.Property(e => e.SumMan)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("sum_man");
            entity.Property(e => e.SumTime)
                .HasColumnType("bigint(20)")
                .HasColumnName("sum_time");
            entity.Property(e => e.Unit).HasColumnName("unit");
        });

        modelBuilder.Entity<AuxFlag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aux_flags")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Alpha2)
                .HasMaxLength(2)
                .HasDefaultValueSql("''")
                .HasColumnName("alpha_2");
            entity.Property(e => e.Alpha3)
                .HasMaxLength(3)
                .HasDefaultValueSql("''")
                .HasColumnName("alpha_3");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("name");
        });

        modelBuilder.Entity<AuxMovement>(entity =>
        {
            entity.HasKey(e => e.IdMovement).HasName("PRIMARY");

            entity.ToTable("aux_movement");

            entity.HasIndex(e => new { e.IdMovement, e.StartTime }, "start_time_and_movement");

            entity.HasIndex(e => new { e.StopTime, e.IdMovement }, "stop_time_and_movement");

            entity.HasIndex(e => e.StartTime, "time");

            entity.Property(e => e.IdMovement)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_movement");
            entity.Property(e => e.BargeField)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(10) unsigned")
                .HasColumnName("barge_field");
            entity.Property(e => e.BunkerField)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .HasColumnName("bunker_field");
            entity.Property(e => e.IdFrom)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_from");
            entity.Property(e => e.IdTo)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_to");
            entity.Property(e => e.IdUserStart)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user_start");
            entity.Property(e => e.IdUserStop)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user_stop");
            entity.Property(e => e.Paused)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("paused");
            entity.Property(e => e.PilotField)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("pilot_field");
            entity.Property(e => e.RemarkField)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .HasColumnName("remark_field");
            entity.Property(e => e.Signature)
                .HasColumnType("bigint(20)")
                .HasColumnName("signature");
            entity.Property(e => e.StartTime)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("start_time");
            entity.Property(e => e.StopTime)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("stop_time");
        });

        modelBuilder.Entity<AuxMovementTug>(entity =>
        {
            entity.HasKey(e => e.IdMovementTugs).HasName("PRIMARY");

            entity.ToTable("aux_movement_tugs");

            entity.HasIndex(e => e.IdMovement, "movement");

            entity.HasIndex(e => new { e.IdTug, e.IdMovement }, "tugs");

            entity.Property(e => e.IdMovementTugs)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id_movement_tugs");
            entity.Property(e => e.IdMovement)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_movement");
            entity.Property(e => e.IdTug)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_tug");
        });

        modelBuilder.Entity<AuxPilot>(entity =>
        {
            entity.HasKey(e => e.IdPilot).HasName("PRIMARY");

            entity.ToTable("aux_pilot");

            entity.HasIndex(e => e.Pilot, "pilot_UNIQUE").IsUnique();

            entity.Property(e => e.IdPilot)
                .HasColumnType("int(11)")
                .HasColumnName("id_pilot");
            entity.Property(e => e.AdminValidated)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("admin_validated");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user");
            entity.Property(e => e.Pilot)
                .HasMaxLength(45)
                .HasColumnName("pilot");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(10) unsigned")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<AuxSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PRIMARY");

            entity.ToTable("aux_settings");

            entity.Property(e => e.SettingId)
                .HasColumnType("int(11)")
                .HasColumnName("setting_id");
            entity.Property(e => e.SettingExplicatie)
                .HasMaxLength(128)
                .HasColumnName("setting_explicatie");
            entity.Property(e => e.SettingName)
                .HasMaxLength(45)
                .HasColumnName("setting_name");
            entity.Property(e => e.SettingValue)
                .HasMaxLength(45)
                .HasColumnName("setting_value");
        });

        modelBuilder.Entity<AuxTug>(entity =>
        {
            entity.HasKey(e => e.IdTug).HasName("PRIMARY");

            entity.ToTable("aux_tugs");

            entity.HasIndex(e => new { e.NameTug, e.IdFlag }, "nume_UNIQUE").IsUnique();

            entity.Property(e => e.IdTug)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_tug");
            entity.Property(e => e.AdminValidated)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("admin_validated");
            entity.Property(e => e.IdFlag)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_flag");
            entity.Property(e => e.IdType)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_type");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user");
            entity.Property(e => e.IdUserChange)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user_change");
            entity.Property(e => e.NameTug)
                .HasMaxLength(45)
                .HasColumnName("name_tug");
            entity.Property(e => e.Remark)
                .HasMaxLength(45)
                .HasColumnName("remark");
            entity.Property(e => e.Timestamp)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<AuxType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("PRIMARY");

            entity.ToTable("aux_type");

            entity.Property(e => e.IdType)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_type");
            entity.Property(e => e.BargeField)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("barge_field");
            entity.Property(e => e.BunkerField)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("bunker_field");
            entity.Property(e => e.CountEvenFreeRunning)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("count_even_free_running");
            entity.Property(e => e.PilotField)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("pilot_field");
            entity.Property(e => e.SsnRequested)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("ssn_requested");
            entity.Property(e => e.Type)
                .HasMaxLength(45)
                .HasColumnName("type");
        });

        modelBuilder.Entity<AuxUser>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.ToTable("aux_users");

            entity.Property(e => e.IdUser)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id_user");
            entity.Property(e => e.AllowEdit)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("allow_edit");
            entity.Property(e => e.IsAdmin)
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("is_admin");
            entity.Property(e => e.Pass)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .HasColumnName("pass");
            entity.Property(e => e.UserFullName)
                .HasMaxLength(45)
                .HasColumnName("user_full_name");
            entity.Property(e => e.UserName)
                .HasMaxLength(45)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<AuxWarn>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("aux_warn");

            entity.HasIndex(e => e.WarnId, "base").IsUnique();

            entity.HasIndex(e => e.Erased, "erased");

            entity.Property(e => e.ClearedByUser)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("cleared_by_user");
            entity.Property(e => e.Erased).HasColumnName("erased");
            entity.Property(e => e.GeneratedByUser)
                .HasColumnType("int(11)")
                .HasColumnName("generated_by_user");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Time)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("time");
            entity.Property(e => e.WarnId)
                .ValueGeneratedOnAdd()
                .HasColumnType("int(10) unsigned")
                .HasColumnName("warn_id");
            entity.Property(e => e.WarnType)
                .HasColumnType("tinyint(4)")
                .HasColumnName("warn_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
