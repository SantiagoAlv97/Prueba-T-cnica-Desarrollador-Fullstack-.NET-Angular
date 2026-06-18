

namespace eventos_vivos.BDO.DTOs.Reservas
{
    public class CrearReservaRequest
    {
        public long EventoId { get; set; }
        public long UsuarioId { get; set; }
        public int Cantidad { get; set; }
    }
}

