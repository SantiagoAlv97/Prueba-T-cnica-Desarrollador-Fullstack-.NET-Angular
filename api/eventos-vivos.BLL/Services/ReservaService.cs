using System.Data;
using eventosvivos.DAL.Persistence;
using eventos_vivos.BDO.DTOs.Reservas;
using eventos_vivos.BDO.Enums;
using eventosvivos.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using eventos_vivos.BLL.Interfaces;

namespace eventos_vivos.BLL.Services
{
    public class ReservaService : IReservaService
    {
        private readonly EventosVivosDbContext _context;

        public ReservaService(EventosVivosDbContext context)
        {
            _context = context;
        }

        public async Task<ReservaResponse> CrearAsync(CrearReservaRequest request)
        {
            if (request.Cantidad < 1) throw new ArgumentException("Cantidad debe ser mayor o igual a 1");

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var evento = await _context.Eventos.FindAsync(request.EventoId);
            if (evento == null) throw new ArgumentException("Evento no existe");

            if (evento.EstadoEventoId == (long)EstadoEventoEnum.Activo && evento.FechaFin <= DateTime.UtcNow)
            {
                evento.EstadoEventoId = (long)EstadoEventoEnum.Completado;
                await _context.SaveChangesAsync();
            }

            if (evento.EstadoEventoId != (long)EstadoEventoEnum.Activo)
                throw new ArgumentException("Solo se pueden reservar entradas para eventos activos");

            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null) throw new ArgumentException("Usuario no existe");

            var ahora = DateTime.UtcNow;
            // RN04: Una reserva ya no debería entrar si al evento le falta menos de 1 hora.
            if (evento.FechaInicio <= ahora.AddHours(1))
                throw new ArgumentException("No se permiten reservas cuando faltan menos de 1 hora para el evento");

            if (evento.FechaInicio < ahora.AddHours(24) && request.Cantidad > 5)
                throw new ArgumentException("Si faltan menos de 24 horas para el evento, solo se permiten máximo 5 entradas por transacción");

            // RN05: para eventos mayores a 100 se limita el número de entradas por transacción.
            if (evento.PrecioEntrada > 100 && request.Cantidad > 10)
                throw new ArgumentException("Para eventos con precio de entrada superior a 100, se permite un máximo de 10 entradas por transacción.");

            var entradasOcupadas = await ObtenerEntradasOcupadasAsync(request.EventoId);
            var entradasDisponibles = evento.CapacidadMaxima - entradasOcupadas;
            if (request.Cantidad > entradasDisponibles)
                throw new ArgumentException("No hay suficientes entradas disponibles");

            var reserva = new Reserva
            {
                EventoId = request.EventoId,
                UsuarioId = request.UsuarioId,
                Cantidad = request.Cantidad,
                NombreComprador = usuario.Nombre,
                EmailComprador = usuario.Email,
                EstadoReservaId = (long)EstadoReservaEnum.PendientePago
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ReservaResponse
            {
                ReservaId = reserva.ReservaId,
                EventoId = reserva.EventoId,
                EventoTitulo = evento.Titulo,
                UsuarioId = reserva.UsuarioId,
                Cantidad = reserva.Cantidad,
                NombreComprador = reserva.NombreComprador,
                EmailComprador = reserva.EmailComprador,
                EstadoReservaId = reserva.EstadoReservaId,
                EstadoReserva = (await _context.EstadoReservas.FindAsync(reserva.EstadoReservaId))?.Descripcion ?? string.Empty,
                CodigoReserva = reserva.CodigoReserva,
                FechaReserva = reserva.FechaReserva
            };
        }

        public async Task<ReservaResponse?> ObtenerPorIdAsync(long id)
        {
            var r = await _context.Reservas
                .Include(x => x.Evento)
                .Include(x => x.Usuario)
                .Include(x => x.EstadoReserva)
                .FirstOrDefaultAsync(x => x.ReservaId == id);
            if (r == null) return null;
            return new ReservaResponse
            {
                ReservaId = r.ReservaId,
                EventoId = r.EventoId,
                EventoTitulo = r.Evento.Titulo,
                UsuarioId = r.UsuarioId,
                Cantidad = r.Cantidad,
                NombreComprador = r.NombreComprador,
                EmailComprador = r.EmailComprador,
                EstadoReservaId = r.EstadoReservaId,
                EstadoReserva = r.EstadoReserva.Descripcion,
                CodigoReserva = r.CodigoReserva,
                FechaReserva = r.FechaReserva,
                FechaPago = r.FechaPago,
                FechaCancelacion = r.FechaCancelacion
            };
        }

        public async Task<IEnumerable<ReservaResponse>> ListarAsync()
        {
            var list = await _context.Reservas
                .Include(x => x.Evento)
                .Include(x => x.Usuario)
                .Include(x => x.EstadoReserva)
                .ToListAsync();

            return list.Select(r => new ReservaResponse
            {
                ReservaId = r.ReservaId,
                EventoId = r.EventoId,
                EventoTitulo = r.Evento.Titulo,
                UsuarioId = r.UsuarioId,
                Cantidad = r.Cantidad,
                NombreComprador = r.NombreComprador,
                EmailComprador = r.EmailComprador,
                EstadoReservaId = r.EstadoReservaId,
                EstadoReserva = r.EstadoReserva.Descripcion,
                CodigoReserva = r.CodigoReserva,
                FechaReserva = r.FechaReserva,
                FechaPago = r.FechaPago,
                FechaCancelacion = r.FechaCancelacion
            }).ToList();
        }

        public async Task<IEnumerable<ReservaResponse>> ListarPorUsuarioAsync(long usuarioId)
        {
            var list = await _context.Reservas
                .Include(x => x.Evento)
                .Include(x => x.Usuario)
                .Include(x => x.EstadoReserva)
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();

            return list.Select(r => new ReservaResponse
            {
                ReservaId = r.ReservaId,
                EventoId = r.EventoId,
                EventoTitulo = r.Evento.Titulo,
                UsuarioId = r.UsuarioId,
                Cantidad = r.Cantidad,
                NombreComprador = r.NombreComprador,
                EmailComprador = r.EmailComprador,
                EstadoReservaId = r.EstadoReservaId,
                EstadoReserva = r.EstadoReserva.Descripcion,
                CodigoReserva = r.CodigoReserva,
                FechaReserva = r.FechaReserva,
                FechaPago = r.FechaPago,
                FechaCancelacion = r.FechaCancelacion
            }).ToList();
        }

        public async Task<ReservaResponse> ConfirmarPagoAsync(long id)
        {
            var r = await _context.Reservas.FindAsync(id);
            if (r == null) throw new ArgumentException("Reserva no existe");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.Confirmada)
                throw new ArgumentException("La reserva ya está confirmada");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.Cancelada)
                throw new ArgumentException("No se puede confirmar una reserva cancelada");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.Perdida)
                throw new ArgumentException("No se puede confirmar una reserva perdida");

            if (r.EstadoReservaId != (long)EstadoReservaEnum.PendientePago)
                throw new ArgumentException("Solo se puede confirmar pago de reservas pendientes");

            r.EstadoReservaId = (long)EstadoReservaEnum.Confirmada;
            r.FechaPago = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(r.CodigoReserva))
                r.CodigoReserva = $"EV-{r.ReservaId.ToString().PadLeft(6, '0')}";

            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(r.ReservaId) ?? throw new Exception("Error al obtener reserva");
        }

        public async Task<ReservaResponse> CancelarAsync(long id)
        {
            var r = await _context.Reservas.Include(x => x.Evento).FirstOrDefaultAsync(x => x.ReservaId == id);
            if (r == null) throw new ArgumentException("Reserva no existe");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.Cancelada)
                throw new ArgumentException("La reserva ya está cancelada");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.Perdida)
                throw new ArgumentException("La reserva ya está perdida");

            if (r.EstadoReservaId == (long)EstadoReservaEnum.PendientePago)
            {
                r.EstadoReservaId = (long)EstadoReservaEnum.Cancelada;
            }
            else if (r.EstadoReservaId == (long)EstadoReservaEnum.Confirmada)
            {
                // RN07: si la reserva ya estaba confirmada y faltan menos de 48 horas, no se libera el cupo; la marco como pérdida para reporte.
                var horasRestantes = r.Evento.FechaInicio - DateTime.UtcNow;
                r.EstadoReservaId = horasRestantes.TotalHours >= 48
                    ? (long)EstadoReservaEnum.Cancelada
                    : (long)EstadoReservaEnum.Perdida;
            }
            else
            {
                throw new ArgumentException("Estado de reserva no válido para cancelación");
            }

            r.FechaCancelacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(r.ReservaId) ?? throw new Exception("Error al obtener reserva");
        }

        private async Task<int> ObtenerEntradasOcupadasAsync(long eventoId)
        {
            // Esto lo estoy contando como ocupado porque sigue pegando en disponibilidad y también en los reportes.
            return await _context.Reservas
                .Where(x =>
                    x.EventoId == eventoId &&
                    (x.EstadoReservaId == (long)EstadoReservaEnum.PendientePago ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Confirmada ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Perdida))
                .SumAsync(x => x.Cantidad);
        }
    }
}
