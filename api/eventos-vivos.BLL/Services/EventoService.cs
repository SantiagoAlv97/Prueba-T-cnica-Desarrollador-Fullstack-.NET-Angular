using eventosvivos.DAL.Persistence;
using eventos_vivos.BDO.DTOs.Eventos;
using eventos_vivos.BDO.Enums;
using eventosvivos.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using eventos_vivos.BLL.Interfaces;

namespace eventos_vivos.BLL.Services
{
    public class EventoService : IEventoService
    {
        private readonly EventosVivosDbContext _context;

        public EventoService(
            EventosVivosDbContext context)
        {
            _context = context;
        }

        public async Task<EventoResponse> CrearAsync(CrearEventoRequest request)
        {
            var venue = await ValidarEventoAsync(
                request.Titulo,
                request.Descripcion,
                request.VenueId,
                request.TipoEventoId,
                request.CapacidadMaxima,
                request.FechaInicio,
                request.FechaFin,
                request.PrecioEntrada,
                (long)EstadoEventoEnum.Activo);

            var tipo = await _context.TipoEventos.FindAsync(request.TipoEventoId);
            if (tipo == null) throw new ArgumentException("Tipo de evento no existe");

            var entity = new Evento
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                VenueId = request.VenueId,
                TipoEventoId = request.TipoEventoId,
                CapacidadMaxima = request.CapacidadMaxima,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                PrecioEntrada = request.PrecioEntrada,
                EstadoEventoId = (long)EstadoEventoEnum.Activo
            };

            _context.Eventos.Add(entity);
            await _context.SaveChangesAsync();

            return new EventoResponse
            {
                EventoId = entity.EventoId,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                VenueId = entity.VenueId,
                VenueNombre = venue.Nombre,
                TipoEventoId = entity.TipoEventoId,
                TipoEvento = tipo.Descripcion,
                CapacidadMaxima = entity.CapacidadMaxima,
                FechaInicio = entity.FechaInicio,
                FechaFin = entity.FechaFin,
                PrecioEntrada = entity.PrecioEntrada,
                EstadoEventoId = entity.EstadoEventoId,
                EstadoEvento = (await _context.EstadoEventos.FindAsync(entity.EstadoEventoId))?.Descripcion ?? string.Empty,
                CuposDisponibles = entity.CapacidadMaxima
            };
        }

        public async Task<bool> ActualizarAsync(long id, ActualizarEventoRequest request)
        {
            var e = await _context.Eventos.FindAsync(id);

            if (e == null) return false;

            if (e.EstadoEventoId == (long)EstadoEventoEnum.Cancelado || e.EstadoEventoId == (long)EstadoEventoEnum.Completado)
                throw new ArgumentException("No se puede editar un evento cancelado o completado");

            await ValidarEventoAsync(
                request.Titulo,
                request.Descripcion,
                request.VenueId,
                request.TipoEventoId,
                request.CapacidadMaxima,
                request.FechaInicio,
                request.FechaFin,
                request.PrecioEntrada,
                request.EstadoEventoId,
                id);

            var estado = await _context.EstadoEventos.FindAsync(request.EstadoEventoId);
            if (estado == null) throw new ArgumentException("Estado de evento no existe");

            var entradasOcupadas = await ObtenerEntradasOcupadasAsync(id);
            if (request.CapacidadMaxima < entradasOcupadas)
                throw new ArgumentException($"La capacidad máxima no puede ser menor que las entradas ya ocupadas ({entradasOcupadas}).");

            e.Titulo = request.Titulo;
            e.Descripcion = request.Descripcion;
            e.VenueId = request.VenueId;
            e.TipoEventoId = request.TipoEventoId;
            e.CapacidadMaxima = request.CapacidadMaxima;
            e.FechaInicio = request.FechaInicio;
            e.FechaFin = request.FechaFin;
            e.PrecioEntrada = request.PrecioEntrada;
            e.EstadoEventoId = request.EstadoEventoId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EventoResponse?> ObtenerPorIdAsync(long id)
        {
            var e = await _context.Eventos
                .Include(x => x.Venue)
                .Include(x => x.TipoEvento)
                .Include(x => x.EstadoEvento)
                .FirstOrDefaultAsync(x => x.EventoId == id);
            if (e == null) return null;

            var entradasOcupadas = await ObtenerEntradasOcupadasAsync(id);

            return new EventoResponse
            {
                EventoId = e.EventoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                VenueId = e.VenueId,
                VenueNombre = e.Venue.Nombre,
                TipoEventoId = e.TipoEventoId,
                TipoEvento = e.TipoEvento.Descripcion,
                CapacidadMaxima = e.CapacidadMaxima,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                PrecioEntrada = e.PrecioEntrada,
                EstadoEventoId = e.EstadoEventoId,
                EstadoEvento = e.EstadoEvento.Descripcion,
                CuposDisponibles = Math.Max(0, e.CapacidadMaxima - entradasOcupadas)
            };
        }

        public async Task<IEnumerable<EventoResponse>> ListarAsync(EventoFiltroRequest filtro)
        {
            await MarcarEventosCompletadosAsync();

            var q = _context.Eventos.AsQueryable();
            if (filtro.TipoEventoId.HasValue) q = q.Where(x => x.TipoEventoId == filtro.TipoEventoId.Value);
            if (filtro.VenueId.HasValue) q = q.Where(x => x.VenueId == filtro.VenueId.Value);
            if (filtro.EstadoEventoId.HasValue) q = q.Where(x => x.EstadoEventoId == filtro.EstadoEventoId.Value);
            if (filtro.FechaDesde.HasValue) q = q.Where(x => x.FechaInicio >= filtro.FechaDesde.Value);
            if (filtro.FechaHasta.HasValue) q = q.Where(x => x.FechaFin <= filtro.FechaHasta.Value);
            if (!string.IsNullOrWhiteSpace(filtro.Titulo))
            {
                var titulo = filtro.Titulo.Trim().ToLower();
                q = q.Where(x => x.Titulo.ToLower().Contains(titulo));
            }

            var list = await q
                .Include(x => x.Venue)
                .Include(x => x.TipoEvento)
                .Include(x => x.EstadoEvento)
                .ToListAsync();

            var eventoIds = list.Select(x => x.EventoId).ToList();
            var entradasOcupadasPorEvento = await _context.Reservas
                .Where(x =>
                    eventoIds.Contains(x.EventoId) &&
                    (x.EstadoReservaId == (long)EstadoReservaEnum.PendientePago ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Confirmada ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Perdida))
                .GroupBy(x => x.EventoId)
                .Select(g => new
                {
                    EventoId = g.Key,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToDictionaryAsync(x => x.EventoId, x => x.Cantidad);

            return list.Select(e => new EventoResponse
            {
                EventoId = e.EventoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                VenueId = e.VenueId,
                VenueNombre = e.Venue.Nombre,
                TipoEventoId = e.TipoEventoId,
                TipoEvento = e.TipoEvento.Descripcion,
                CapacidadMaxima = e.CapacidadMaxima,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                PrecioEntrada = e.PrecioEntrada,
                EstadoEventoId = e.EstadoEventoId,
                EstadoEvento = e.EstadoEvento.Descripcion,
                CuposDisponibles = Math.Max(
                    0,
                    e.CapacidadMaxima - entradasOcupadasPorEvento.GetValueOrDefault(e.EventoId, 0)
                )
            }).ToList();
        }

        public async Task<bool> CancelarAsync(long id)
        {
            var e = await _context.Eventos.FindAsync(id);
            if (e == null) return false;
            e.EstadoEventoId = (long)EstadoEventoEnum.Cancelado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TipoEventoResponse>> ListarTiposAsync()
        {
            var list = await _context.TipoEventos.ToListAsync();
            return list.Select(t => new TipoEventoResponse
            {
                TipoEventoId = t.TipoEventoId,
                Descripcion = t.Descripcion
            }).ToList();
        }

        public async Task<IEnumerable<EstadoEventoResponse>> ListarEstadosAsync()
        {
            var list = await _context.EstadoEventos.ToListAsync();
            return list.Select(t => new EstadoEventoResponse
            {
                EstadoEventoId = t.EstadoEventoId,
                Descripcion = t.Descripcion
            }).ToList();
        }

        private async Task<Venue> ValidarEventoAsync(
            string titulo,
            string descripcion,
            long venueId,
            long tipoEventoId,
            int capacidadMaxima,
            DateTime fechaInicio,
            DateTime fechaFin,
            decimal precioEntrada,
            long estadoEventoId,
            long? eventoId = null)
        {
            if (string.IsNullOrWhiteSpace(titulo) || titulo.Length < 5 || titulo.Length > 100)
                throw new ArgumentException("Título inválido");

            if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length < 10 || descripcion.Length > 500)
                throw new ArgumentException("Descripción inválida. Debe tener entre 10 y 500 caracteres.\r\n");

            if (venueId <= 0) throw new ArgumentException("Venue es obligatorio");

            if (tipoEventoId <= 0) throw new ArgumentException("Tipo de evento es obligatorio");

            if (capacidadMaxima <= 0) throw new ArgumentException("Capacidad máxima debe ser mayor a 0");

            if (fechaInicio <= DateTime.UtcNow) throw new ArgumentException("Fecha inicio debe ser futura");

            if (fechaFin <= fechaInicio) throw new ArgumentException("Fecha fin debe ser mayor a fecha inicio");

            if (precioEntrada <= 0) throw new ArgumentException("Precio entrada debe ser mayor a 0");

            // RN03: preferí validar el horario nocturno aquí porque el horario nace desde la creación o la edición del evento.
            if ((fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday) &&
                fechaInicio.TimeOfDay > TimeSpan.FromHours(22))
                throw new ArgumentException("Los eventos de fin de semana no pueden iniciar después de las 22:00");

            var venue = await _context.Venues.FindAsync(venueId);
            if (venue == null) throw new ArgumentException("Venue no existe");
            // RN01: el evento no puede quedar por encima de la capacidad real del venue asignado.
            if (capacidadMaxima > venue.Capacidad)
                throw new ArgumentException("Capacidad máxima no puede ser mayor a la capacidad del venue");

            var tipo = await _context.TipoEventos.FindAsync(tipoEventoId);
            if (tipo == null) throw new ArgumentException("Tipo de evento no existe");

            if (estadoEventoId == (long)EstadoEventoEnum.Activo)
            {
                // RN02: si el evento queda activo, reviso de una vez que no se cruce con otro activo en el mismo venue.
                var existeConflicto = await _context.Eventos.AnyAsync(x =>
                    x.EventoId != eventoId &&
                    x.VenueId == venueId &&
                    x.EstadoEventoId == (long)EstadoEventoEnum.Activo &&
                    fechaInicio < x.FechaFin &&
                    fechaFin > x.FechaInicio);

                if (existeConflicto)
                    throw new ArgumentException("Ya existe un evento activo en el mismo venue con horario superpuesto");
            }

            return venue;
        }

        private async Task<int> ObtenerEntradasOcupadasAsync(long eventoId)
        {
            return await _context.Reservas
                .Where(x =>
                    x.EventoId == eventoId &&
                    (x.EstadoReservaId == (long)EstadoReservaEnum.PendientePago ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Confirmada ||
                     x.EstadoReservaId == (long)EstadoReservaEnum.Perdida))
                .SumAsync(x => x.Cantidad);
        }

        private async Task MarcarEventosCompletadosAsync()
        {
            // RN06: Este ajuste automático evita que el estado del evento se quede activo cuando ya pasó a la hora real de fin.
            var eventos = await _context.Eventos
                .Where(x =>
                    x.EstadoEventoId == (long)EstadoEventoEnum.Activo &&
                    x.FechaFin <= DateTime.UtcNow)
                .ToListAsync();

            if (!eventos.Any()) return;

            foreach (var evento in eventos)
            {
                evento.EstadoEventoId = (long)EstadoEventoEnum.Completado;
            }

            await _context.SaveChangesAsync();
        }
    }
}

