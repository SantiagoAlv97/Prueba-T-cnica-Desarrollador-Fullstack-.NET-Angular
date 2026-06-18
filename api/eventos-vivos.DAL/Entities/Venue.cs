using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class Venue
{
    public long VenueId { get; set; }

    public string Nombre { get; set; } = null!;

    public int Capacidad { get; set; }

    public string Ciudad { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
