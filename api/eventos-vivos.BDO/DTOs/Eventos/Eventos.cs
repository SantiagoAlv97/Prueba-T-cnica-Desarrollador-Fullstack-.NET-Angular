using System;
using System.Collections.Generic;
using System.Text;

namespace eventos_vivos.BDO.DTOs.Eventos
{
    public class CrearEventoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public long VenueId { get; set; }
        public long TipoEventoId { get; set; }
        public int CapacidadMaxima { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioEntrada { get; set; }
    }

    public class ActualizarEventoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public long VenueId { get; set; }
        public long TipoEventoId { get; set; }
        public int CapacidadMaxima { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioEntrada { get; set; }
        public long EstadoEventoId { get; set; }
    }

    public class EventoResponse
    {
        public long EventoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public long VenueId { get; set; }
        public string VenueNombre { get; set; } = string.Empty;
        public long TipoEventoId { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioEntrada { get; set; }
        public long EstadoEventoId { get; set; }
        public string EstadoEvento { get; set; } = string.Empty;
        public int CuposDisponibles { get; set; }
    }

    public class EventoFiltroRequest
    {
        public long? TipoEventoId { get; set; }
        public long? VenueId { get; set; }
        public long? EstadoEventoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Titulo { get; set; }
    }

    public class TipoEventoResponse
    {
        public long TipoEventoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class EstadoEventoResponse
    {
        public long EstadoEventoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
