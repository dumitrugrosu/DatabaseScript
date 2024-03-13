using System;
using System.Collections.Generic;
using DatabaseScript.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseScript.Context;

public partial class AuxVesselsContext : DbContext
{
    public AuxVesselsContext()
    {
    }

    public AuxVesselsContext(DbContextOptions<AuxVesselsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuxBarge> AuxBarges { get; set; }

    public virtual DbSet<AuxEstimatedTime> AuxEstimatedTimes { get; set; }

    public virtual DbSet<AuxManeuver> AuxManeuvers { get; set; }

    public virtual DbSet<AuxPilot> AuxPilots { get; set; }

    public virtual DbSet<AuxTug> AuxTugs { get; set; }

    public virtual DbSet<MngAuxBarges> MngAuxBarges { get; set; }

    public virtual DbSet<MngAuxPilot> MngAuxPilots { get; set; }

    public virtual DbSet<MngAuxTug> MngAuxTugs { get; set; }

    public virtual DbSet<MngAuxTugType> MngAuxTugTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DUMITRU;Database=aux_vessels;Integrated Security=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuxBarge>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.AUX_BARGES");

            entity.ToTable("AUX_BARGES");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.AuxSid).HasColumnName("AUX_SID");
            entity.Property(e => e.BargeSid).HasColumnName("BARGE_SID");

            entity.HasOne(d => d.AuxS).WithMany(p => p.AuxBarges)
                .HasForeignKey(d => d.AuxSid)
                .HasConstraintName("FK_dbo.AUX_BARGES_dbo.AUX_MANEUVERS_AUX_SID");

            entity.HasOne(d => d.BargeS).WithMany(p => p.AuxBarges)
                .HasForeignKey(d => d.BargeSid)
                .HasConstraintName("FK_dbo.AUX_BARGES_dbo.MNG_AUX_BARGES_BARGE_SID");
        });

        modelBuilder.Entity<AuxEstimatedTime>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.AUX_ESTIMATED_TIME");

            entity.ToTable("AUX_ESTIMATED_TIME");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.FromBerth)
                .HasMaxLength(128)
                .HasColumnName("FROM_BERTH");
            entity.Property(e => e.LastRegisterTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_REGISTER_TIME");
            entity.Property(e => e.SumMan).HasColumnName("SUM_MAN");
            entity.Property(e => e.SumTimeSec).HasColumnName("SUM_TIME_SEC");
            entity.Property(e => e.ToBerth)
                .HasMaxLength(128)
                .HasColumnName("TO_BERTH");
        });

        modelBuilder.Entity<AuxManeuver>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.AUX_MANEUVERS");

            entity.ToTable("AUX_MANEUVERS");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.BunkerFieldImo)
                .HasMaxLength(7)
                .HasColumnName("BUNKER_FIELD_IMO");
            entity.Property(e => e.BunkerFieldName).HasColumnName("BUNKER_FIELD_NAME");
            entity.Property(e => e.ConfirmPortChange).HasColumnName("CONFIRM_PORT_CHANGE");
            entity.Property(e => e.Details).HasColumnName("DETAILS");
            entity.Property(e => e.FromPort).HasColumnName("FROM_PORT");
            entity.Property(e => e.FromPosition).HasColumnName("FROM_POSITION");
            entity.Property(e => e.FromTime)
                .HasColumnType("datetime")
                .HasColumnName("FROM_TIME");
            entity.Property(e => e.Index).HasColumnName("INDEX");
            entity.Property(e => e.PausedSec).HasColumnName("PAUSED_SEC");
            entity.Property(e => e.PausedTime)
                .HasColumnType("datetime")
                .HasColumnName("PAUSED_TIME");
            entity.Property(e => e.RegisterTime)
                .HasColumnType("datetime")
                .HasColumnName("REGISTER_TIME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.ToPort).HasColumnName("TO_PORT");
            entity.Property(e => e.ToPosition).HasColumnName("TO_POSITION");
            entity.Property(e => e.ToTime)
                .HasColumnType("datetime")
                .HasColumnName("TO_TIME");
            entity.Property(e => e.Type).HasColumnName("TYPE");
            entity.Property(e => e.UserEndSid).HasColumnName("USER_END_SID");
            entity.Property(e => e.UserSid).HasColumnName("USER_SID");
        });

        modelBuilder.Entity<AuxPilot>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.AUX_PILOTS");

            entity.ToTable("AUX_PILOTS");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.AuxSid).HasColumnName("AUX_SID");
            entity.Property(e => e.PilotSid).HasColumnName("PILOT_SID");

            entity.HasOne(d => d.AuxS).WithMany(p => p.AuxPilots)
                .HasForeignKey(d => d.AuxSid)
                .HasConstraintName("FK_dbo.AUX_PILOTS_dbo.AUX_MANEUVERS_AUX_SID");

            entity.HasOne(d => d.PilotS).WithMany(p => p.AuxPilots)
                .HasForeignKey(d => d.PilotSid)
                .HasConstraintName("FK_dbo.AUX_PILOTS_dbo.MNG_AUX_PILOTS_PILOT_SID");
        });

        modelBuilder.Entity<AuxTug>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.AUX_TUGS");

            entity.ToTable("AUX_TUGS");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.AuxSid).HasColumnName("AUX_SID");
            entity.Property(e => e.TugSid).HasColumnName("TUG_SID");

            entity.HasOne(d => d.AuxS).WithMany(p => p.AuxTugs)
                .HasForeignKey(d => d.AuxSid)
                .HasConstraintName("FK_dbo.AUX_TUGS_dbo.AUX_MANEUVERS_AUX_SID");

            entity.HasOne(d => d.TugS).WithMany(p => p.AuxTugs)
                .HasForeignKey(d => d.TugSid)
                .HasConstraintName("FK_dbo.AUX_TUGS_dbo.MNG_AUX_TUGS_TUG_SID");
        });

        modelBuilder.Entity<MngAuxBarges>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.MNG_AUX_BARGES");

            entity.ToTable("MNG_AUX_BARGES");

            entity.Property(e => e.Sid).HasColumnName("SID").ValueGeneratedOnAdd();
            entity.Property(e => e.BargeName).HasColumnName("BARGE_NAME");
            entity.Property(e => e.BargeStatus).HasColumnName("BARGE_STATUS");
            entity.Property(e => e.Details).HasColumnName("DETAILS");
            entity.Property(e => e.MmsiNumber).HasColumnName("MMSI_NUMBER");
        });

        modelBuilder.Entity<MngAuxPilot>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.MNG_AUX_PILOTS");

            entity.ToTable("MNG_AUX_PILOTS");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Category).HasColumnName("CATEGORY");
            entity.Property(e => e.CompanySid).HasColumnName("COMPANY_SID");
            entity.Property(e => e.Details).HasColumnName("DETAILS");
            entity.Property(e => e.PilotName).HasColumnName("PILOT_NAME");
            entity.Property(e => e.PilotStatus).HasColumnName("PILOT_STATUS");
        });

        modelBuilder.Entity<MngAuxTug>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.MNG_AUX_TUGS");

            entity.ToTable("MNG_AUX_TUGS");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.AuxTypeSid).HasColumnName("AUX_TYPE_SID");
            entity.Property(e => e.Barge).HasColumnName("BARGE");
            entity.Property(e => e.Bunker).HasColumnName("BUNKER");
            entity.Property(e => e.CountFree).HasColumnName("COUNT_FREE");
            entity.Property(e => e.Details).HasColumnName("DETAILS");
            entity.Property(e => e.TugFlagState)
                .HasMaxLength(2)
                .HasColumnName("TUG_FLAG_STATE");
            entity.Property(e => e.TugName).HasColumnName("TUG_NAME");
            entity.Property(e => e.TugStatus).HasColumnName("TUG_STATUS");

            entity.HasOne(d => d.AuxTypeS).WithMany(p => p.MngAuxTugs)
                .HasForeignKey(d => d.AuxTypeSid)
                .HasConstraintName("FK_dbo.MNG_AUX_TUGS_dbo.MNG_AUX_TUG_TYPE_AUX_TYPE_SID");
        });

        modelBuilder.Entity<MngAuxTugType>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK_dbo.MNG_AUX_TUG_TYPE");

            entity.ToTable("MNG_AUX_TUG_TYPE");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Barge).HasColumnName("BARGE");
            entity.Property(e => e.Bunker).HasColumnName("BUNKER");
            entity.Property(e => e.CountFree).HasColumnName("COUNT_FREE");
            entity.Property(e => e.Details).HasColumnName("DETAILS");
            entity.Property(e => e.Ssn).HasColumnName("SSN");
            entity.Property(e => e.Type).HasColumnName("TYPE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
