using System.Collections.Generic;
using System.Threading.Tasks;
using eventos_vivos.BDO.DTOs.Venues;

namespace eventos_vivos.BLL.Interfaces
{
    public interface IVenueService
    {
        Task<IEnumerable<VenueResponse>> ListarAsync();
        Task<VenueResponse?> ObtenerPorIdAsync(long id);
        Task<VenueResponse> CrearAsync(CrearVenueRequest request);
        Task<bool> ActualizarAsync(long id, ActualizarVenueRequest request);
        Task<bool> EliminarAsync(long id);
    }
}
