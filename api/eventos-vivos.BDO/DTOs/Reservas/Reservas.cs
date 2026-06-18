
using System.Text.Json.Serialization;
namespace eventos_vivos.BDO.DTOs.Reservas
{
    public class CrearReservaRequest
    {
        public long EventoId { get; set; }
        [JsonIgnore]
        public long UsuarioId { get; set; }
        public int Cantidad { get; set; }
    }
}


