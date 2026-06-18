using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class Evento
{
    public long EventoId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public long VenueId { get; set; }

    public long TipoEventoId { get; set; }

    public int CapacidadMaxima { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public decimal PrecioEntrada { get; set; }

    public long EstadoEventoId { get; set; }

    public virtual EstadoEvento EstadoEvento { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual TipoEvento TipoEvento { get; set; } = null!;

    public virtual Venue Venue { get; set; } = null!;
}
