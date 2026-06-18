using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class Usuario
{
    public long UsuarioId { get; set; }

    public string GoogleId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? FotoUrl { get; set; }

    public long RolId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaUltimoAcceso { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Role Rol { get; set; } = null!;
}
