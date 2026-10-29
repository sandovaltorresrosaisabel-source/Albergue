using System;
using System.Collections.Generic;
using Albergue.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Albergue.Api.Data;

public partial class AlbergueContext : DbContext
{
    public AlbergueContext()
    {
    }

    public AlbergueContext(DbContextOptions<AlbergueContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SolicitudAdopcion> SolicitudAdopcions { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.IdAnimal).HasName("PK__Animal__951092F06D73B3E9");

            entity.ToTable("Animal");

            entity.Property(e => e.Especie).HasMaxLength(30);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Disponible");
            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584CC9007463");

            entity.ToTable("Rol");

            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<SolicitudAdopcion>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__Solicitu__36899CEF7BB686C5");

            entity.ToTable("SolicitudAdopcion");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Enviada");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdAnimalNavigation).WithMany(p => p.SolicitudAdopcions)
                .HasForeignKey(d => d.IdAnimal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__IdAni__5535A963");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SolicitudAdopcions)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__IdUsu__5441852A");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF974ADA61A4");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D1053479236149").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__IdRol__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
