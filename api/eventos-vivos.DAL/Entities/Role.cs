using System;
using System.Collections.Generic;

namespace eventosvivos.DAL.Entities;

public partial class Role
{
    public long RolId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
