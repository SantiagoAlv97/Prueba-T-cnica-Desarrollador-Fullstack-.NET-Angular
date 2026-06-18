using System.Threading.Tasks;
using eventos_vivos.BDO.DTOs.Reportes;

namespace eventos_vivos.BLL.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteOcupacionResponse?> ObtenerReporteOcupacionAsync(long eventoId);
    }
}
