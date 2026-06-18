using eventosvivos.DAL.Persistence;
using eventos_vivos.BDO.DTOs.Venues;
using eventosvivos.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using eventos_vivos.BLL.Interfaces;

namespace eventos_vivos.BLL.Services
{
    public class VenueService : IVenueService
    {
        private readonly EventosVivosDbContext _context;

        public VenueService(EventosVivosDbContext context)
        {
            _context = context;
        }

        public async Task<VenueResponse> CrearAsync(CrearVenueRequest request)
        {
            var entity = new Venue
            {
                Nombre = request.Nombre,
                Capacidad = request.Capacidad,
                Ciudad = request.Ciudad
            };
            _context.Venues.Add(entity);
            await _context.SaveChangesAsync();

            return new VenueResponse
            {
                VenueId = entity.VenueId,
                Nombre = entity.Nombre,
                Capacidad = entity.Capacidad,
                Ciudad = entity.Ciudad
            };
        }

        public async Task<bool> EliminarAsync(long id)
        {
            var venue = await _context.Venues.Include(v => v.Eventos).FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return false;
            if (venue.Eventos != null && venue.Eventos.Any())
            {
                throw new System.Exception("No se puede eliminar un venue que tiene eventos asociados.");
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VenueResponse?> ObtenerPorIdAsync(long id)
        {
            var v = await _context.Venues.FirstOrDefaultAsync(x => x.VenueId == id);
            if (v == null) return null;
            return new VenueResponse
            {
                VenueId = v.VenueId,
                Nombre = v.Nombre,
                Capacidad = v.Capacidad,
                Ciudad = v.Ciudad
            };
        }

        public async Task<IEnumerable<VenueResponse>> ListarAsync()
        {
            return await _context.Venues
                .Select(v => new VenueResponse
                {
                    VenueId = v.VenueId,
                    Nombre = v.Nombre,
                    Capacidad = v.Capacidad,
                    Ciudad = v.Ciudad
                }).ToListAsync();
        }

        public async Task<bool> ActualizarAsync(long id, ActualizarVenueRequest request)
        {
            var v = await _context.Venues.FirstOrDefaultAsync(x => x.VenueId == id);
            if (v == null) return false;
            v.Nombre = request.Nombre;
            v.Capacidad = request.Capacidad;
            v.Ciudad = request.Ciudad;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
