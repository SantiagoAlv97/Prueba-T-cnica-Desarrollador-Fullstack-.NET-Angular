using System;

namespace eventos_vivos.BDO.DTOs.Reservas
{
    public class ReservaResponse
    {
        public long ReservaId { get; set; }
        public long EventoId { get; set; }
        public string EventoTitulo { get; set; } = string.Empty;
        public long UsuarioId { get; set; }
        public int Cantidad { get; set; }
        public string NombreComprador { get; set; } = string.Empty;
        public string EmailComprador { get; set; } = string.Empty;
        public long EstadoReservaId { get; set; }
        public string EstadoReserva { get; set; } = string.Empty;
        public string? CodigoReserva { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime? FechaCancelacion { get; set; }
    }
}
