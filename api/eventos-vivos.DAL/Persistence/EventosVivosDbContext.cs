using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eventosvivos.DAL.Entities;

namespace eventosvivos.DAL.Persistence;

public partial class EventosVivosDbContext : DbContext
{
    public EventosVivosDbContext(DbContextOptions<EventosVivosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EstadoEvento> EstadoEventos { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReservas { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TipoEvento> TipoEventos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EstadoEvento>(entity =>
        {
            entity.HasKey(e => e.EstadoEventoId).HasName("PK__EstadoEv__35689259E041B2AA");

            entity.ToTable("EstadoEvento");

            entity.HasIndex(e => e.Descripcion, "UQ__EstadoEv__92C53B6CF258E08C").IsUnique();

            entity.Property(e => e.EstadoEventoId)
                .ValueGeneratedNever()
                .HasColumnName("EstadoEventoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasKey(e => e.EstadoReservaId).HasName("PK__EstadoRe__DB6E9F017CFA3E67");

            entity.ToTable("EstadoReserva");

            entity.HasIndex(e => e.Descripcion, "UQ__EstadoRe__92C53B6C22396ABA").IsUnique();

            entity.Property(e => e.EstadoReservaId)
                .ValueGeneratedNever()
                .HasColumnName("EstadoReservaID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.EventoId).HasName("PK__Eventos__1EEB59016664F5E7");

            entity.HasIndex(e => e.FechaInicio, "IX_Eventos_FechaInicio");

            entity.HasIndex(e => e.VenueId, "IX_Eventos_VenueID");

            entity.Property(e => e.EventoId).HasColumnName("EventoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EstadoEventoId)
                .HasDefaultValue(1L)
                .HasColumnName("EstadoEventoID");
            entity.Property(e => e.PrecioEntrada).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoEventoId).HasColumnName("TipoEventoID");
            entity.Property(e => e.Titulo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VenueId).HasColumnName("VenueID");

            entity.HasOne(d => d.EstadoEvento).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.EstadoEventoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_EstadoEvento");

            entity.HasOne(d => d.TipoEvento).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.TipoEventoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_TipoEvento");

            entity.HasOne(d => d.Venue).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Eventos_Venues");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PK__Reservas__C3993703E6B035AD");

            entity.HasIndex(e => e.CodigoReserva, "IX_Reservas_CodigoReserva")
                .IsUnique()
                .HasFilter("([CodigoReserva] IS NOT NULL)");

            entity.HasIndex(e => e.EventoId, "IX_Reservas_EventoID");

            entity.HasIndex(e => e.UsuarioId, "IX_Reservas_UsuarioID");

            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.CodigoReserva)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EmailComprador)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.EstadoReservaId)
                .HasDefaultValue(1L)
                .HasColumnName("EstadoReservaID");
            entity.Property(e => e.EventoId).HasColumnName("EventoID");
            entity.Property(e => e.FechaReserva).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.NombreComprador)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.EstadoReserva).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EstadoReservaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_EstadoReserva");

            entity.HasOne(d => d.Evento).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EventoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Eventos");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Usuarios");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302D17008137E");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__75E3EFCF36E2E6DE").IsUnique();

            entity.Property(e => e.RolId)
                .ValueGeneratedNever()
                .HasColumnName("RolID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoEvento>(entity =>
        {
            entity.HasKey(e => e.TipoEventoId).HasName("PK__TipoEven__4231C163433956C3");

            entity.ToTable("TipoEvento");

            entity.HasIndex(e => e.Descripcion, "UQ__TipoEven__92C53B6C5161EB0D").IsUnique();

            entity.Property(e => e.TipoEventoId).HasColumnName("TipoEventoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE79811D0BED6");

            entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

            entity.HasIndex(e => e.GoogleId, "UQ_Usuarios_GoogleID").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.GoogleId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("GoogleID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RolId)
                .HasDefaultValue(1L)
                .HasColumnName("RolID");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5D29D15D65E");

            entity.Property(e => e.VenueId).HasColumnName("VenueID");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
