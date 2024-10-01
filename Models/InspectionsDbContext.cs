using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RPBDISLAB2.Views;

namespace RPBDISLAB2.Models;

public partial class InspectionsDbContext : DbContext
{
    public InspectionsDbContext()
    {
    }

    public InspectionsDbContext(DbContextOptions<InspectionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Enterprise> Enterprises { get; set; }

    public virtual DbSet<Inspection> Inspections { get; set; }

    public virtual DbSet<Inspector> Inspectors { get; set; }

    public virtual DbSet<VInspectorWork> VInspectorWorks { get; set; }

    public virtual DbSet<VOffendingEnterprise> VOffendingEnterprises { get; set; }

    public virtual DbSet<VPenaltyDetail> VPenaltyDetails { get; set; }

    public virtual DbSet<ViolationType> ViolationTypes { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enterprise>(entity =>
        {
            entity.HasKey(e => e.EnterpriseId).HasName("PK__Enterpri__52DEA546513AC6E4");

            entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.DirectorName).HasMaxLength(100);
            entity.Property(e => e.DirectorPhone).HasMaxLength(15);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OwnershipType).HasMaxLength(50);
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.InspectionId).HasName("PK__Inspecti__30B2DC2801B3D1BE");

            entity.Property(e => e.InspectionId).HasColumnName("InspectionID");
            entity.Property(e => e.CorrectionStatus).HasMaxLength(50);
            entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseID");
            entity.Property(e => e.InspectorId).HasColumnName("InspectorID");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.PenaltyAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProtocolNumber).HasMaxLength(50);
            entity.Property(e => e.ResponsiblePerson).HasMaxLength(100);
            entity.Property(e => e.ViolationTypeId).HasColumnName("ViolationTypeID");

            entity.HasOne(d => d.Enterprise).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.EnterpriseId)
                .HasConstraintName("FK_Inspections_Enterprises");

            entity.HasOne(d => d.Inspector).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.InspectorId)
                .HasConstraintName("FK_Inspections_Inspectors");

            entity.HasOne(d => d.ViolationType).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.ViolationTypeId)
                .HasConstraintName("FK_Inspections_ViolationTypes");
        });

        modelBuilder.Entity<Inspector>(entity =>
        {
            entity.HasKey(e => e.InspectorId).HasName("PK__Inspecto__5FECC3FD7E3AA5EA");

            entity.Property(e => e.InspectorId).HasColumnName("InspectorID");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        modelBuilder.Entity<VInspectorWork>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_InspectorWork");

            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.InspectedEnterprises).HasMaxLength(4000);
            entity.Property(e => e.InspectorName).HasMaxLength(100);
            entity.Property(e => e.TotalPenaltyAmount).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VOffendingEnterprise>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_OffendingEnterprises");

            entity.Property(e => e.CorrectionStatus)
                .HasMaxLength(28)
                .IsUnicode(false);
            entity.Property(e => e.DirectorName).HasMaxLength(100);
            entity.Property(e => e.DirectorPhone).HasMaxLength(15);
            entity.Property(e => e.EnterpriseName).HasMaxLength(100);
            entity.Property(e => e.ViolationList).HasMaxLength(4000);
        });

        modelBuilder.Entity<VPenaltyDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_PenaltyDetails");

            entity.Property(e => e.EnterpriseName).HasMaxLength(100);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.PenaltyAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ViolationName).HasMaxLength(100);
        });

        modelBuilder.Entity<ViolationType>(entity =>
        {
            entity.HasKey(e => e.ViolationTypeId).HasName("PK__Violatio__3B1A4D7DD943DDEB");

            entity.Property(e => e.ViolationTypeId).HasColumnName("ViolationTypeID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PenaltyAmount).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
