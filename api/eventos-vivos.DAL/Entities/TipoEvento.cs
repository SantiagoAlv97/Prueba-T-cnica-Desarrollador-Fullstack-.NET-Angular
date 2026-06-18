using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class TipoEvento
{
    public long TipoEventoId { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
