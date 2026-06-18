using System.Collections.Generic;
using System.Threading.Tasks;
using eventos_vivos.BDO.DTOs.Eventos;

namespace eventos_vivos.BLL.Interfaces
{
    public interface IEventoService
    {
        Task<IEnumerable<EventoResponse>> ListarAsync(EventoFiltroRequest filtro);
        Task<IEnumerable<TipoEventoResponse>> ListarTiposAsync();
        Task<IEnumerable<EstadoEventoResponse>> ListarEstadosAsync();
        Task<EventoResponse?> ObtenerPorIdAsync(long id);
        Task<EventoResponse> CrearAsync(CrearEventoRequest request);
        Task<bool> ActualizarAsync(long id, ActualizarEventoRequest request);
        Task<bool> CancelarAsync(long id);
    }
}
