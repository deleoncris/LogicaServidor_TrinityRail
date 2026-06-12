using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LogicaServidor.Models.Entities;

public partial class SensoresTrinityContext : DbContext
{
    public SensoresTrinityContext(DbContextOptions<SensoresTrinityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Datos> Datos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Datos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Co2)
                .HasColumnType("int(11)")
                .HasColumnName("CO2");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Humedad).HasColumnType("int(11)");
            entity.Property(e => e.NumeroSerie).HasMaxLength(20);
            entity.Property(e => e.Temperatura).HasColumnType("int(11)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
