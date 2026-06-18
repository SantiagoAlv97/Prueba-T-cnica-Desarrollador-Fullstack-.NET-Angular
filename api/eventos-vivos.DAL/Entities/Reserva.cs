using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class Reserva
{
    public long ReservaId { get; set; }

    public long EventoId { get; set; }

    public long UsuarioId { get; set; }

    public int Cantidad { get; set; }

    public string NombreComprador { get; set; } = null!;

    public string EmailComprador { get; set; } = null!;

    public long EstadoReservaId { get; set; }

    public string? CodigoReserva { get; set; }

    public DateTime FechaReserva { get; set; }

    public DateTime? FechaPago { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public virtual EstadoReserva EstadoReserva { get; set; } = null!;

    public virtual Evento Evento { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
