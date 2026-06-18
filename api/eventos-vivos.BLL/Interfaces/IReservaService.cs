using System.Collections.Generic;
using System.Threading.Tasks;
using eventos_vivos.BDO.DTOs.Reservas;

namespace eventos_vivos.BLL.Interfaces
{
    public interface IReservaService
    {
        Task<IEnumerable<ReservaResponse>> ListarAsync();
        Task<IEnumerable<ReservaResponse>> ListarPorUsuarioAsync(long usuarioId);
        Task<ReservaResponse?> ObtenerPorIdAsync(long id);
        Task<ReservaResponse> CrearAsync(CrearReservaRequest request);
        Task<ReservaResponse> ConfirmarPagoAsync(long id);
        Task<ReservaResponse> CancelarAsync(long id);
    }
}
