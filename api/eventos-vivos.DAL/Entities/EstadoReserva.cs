using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class EstadoReserva
{
    public long EstadoReservaId { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
