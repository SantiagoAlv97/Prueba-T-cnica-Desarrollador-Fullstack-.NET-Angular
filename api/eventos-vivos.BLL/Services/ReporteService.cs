using System.Linq;
using System.Threading.Tasks;
using eventosvivos.DAL.Persistence;
using eventos_vivos.BDO.DTOs.Reportes;
using eventos_vivos.BDO.Enums;
using Microsoft.EntityFrameworkCore;
using eventos_vivos.BLL.Interfaces;

namespace eventos_vivos.BLL.Services
{
    public class ReporteService : IReporteService
    {
        private readonly EventosVivosDbContext _context;

        public ReporteService(EventosVivosDbContext context)
        {
            _context = context;
        }

        public async Task<ReporteOcupacionResponse?> ObtenerReporteOcupacionAsync(long eventoId)
        {
            var e = await _context.Eventos.Include(x => x.EstadoEvento).FirstOrDefaultAsync(x => x.EventoId == eventoId);
            if (e == null) return null;

            if (e.EstadoEventoId == (long)EstadoEventoEnum.Activo && e.FechaFin <= System.DateTime.UtcNow)
            {
                e.EstadoEventoId = (long)EstadoEventoEnum.Completado;
                await _context.SaveChangesAsync();
                e = await _context.Eventos.Include(x => x.EstadoEvento).FirstOrDefaultAsync(x => x.EventoId == eventoId);
                if (e == null) return null;
            }

            var reservas = await _context.Reservas.Where(r => r.EventoId == eventoId).ToListAsync();
            var entradasConfirmadas = reservas.Where(r => r.EstadoReservaId == (long)EstadoReservaEnum.Confirmada).Sum(r => r.Cantidad);
            var entradasPendientes = reservas.Where(r => r.EstadoReservaId == (long)EstadoReservaEnum.PendientePago).Sum(r => r.Cantidad);
            var entradasPerdidas = reservas.Where(r => r.EstadoReservaId == (long)EstadoReservaEnum.Perdida).Sum(r => r.Cantidad);

            var entradasDisponibles = e.CapacidadMaxima - entradasConfirmadas - entradasPendientes - entradasPerdidas;
            if (entradasDisponibles < 0) entradasDisponibles = 0;

            var porcentaje = e.CapacidadMaxima == 0 ? 0 : (decimal)entradasConfirmadas / e.CapacidadMaxima * 100;
            var totalIngresos = e.PrecioEntrada * entradasConfirmadas;

            return new ReporteOcupacionResponse
            {
                EventoId = e.EventoId,
                EventoTitulo = e.Titulo,
                CapacidadMaxima = e.CapacidadMaxima,
                EntradasVendidas = entradasConfirmadas,
                EntradasDisponibles = entradasDisponibles,
                PorcentajeOcupacion = porcentaje,
                TotalIngresos = totalIngresos,
                EstadoEvento = e.EstadoEvento.Descripcion
            };
        }
    }
}
