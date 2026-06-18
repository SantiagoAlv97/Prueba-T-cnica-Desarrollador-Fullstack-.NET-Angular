using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class EstadoEvento
{
    public long EstadoEventoId { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
