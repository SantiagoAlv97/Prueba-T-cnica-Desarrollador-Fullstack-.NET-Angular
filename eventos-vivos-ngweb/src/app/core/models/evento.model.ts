export interface Evento {
  id: number | string;
  eventoId?: number | string;
  titulo: string;
  descripcion?: string | null;
  venueId?: number | string | null;
  tipoEventoId?: number | string | null;
  capacidadMaxima?: number | string | null;
  fechaInicio?: string | null;
  fechaFin?: string | null;
  precioEntrada?: number | string | null;
  estadoEventoId?: number | string | null;
  venue?: VenueResumen | string | null;
  venueNombre?: string | null;
  ciudad?: string | null;
  tipoEvento?: TipoEventoResumen | string | null;
  tipoEventoNombre?: string | null;
  estadoEvento?: EstadoEventoResumen | string | null;
  estadoEventoNombre?: string | null;
  cuposDisponibles?: number | string | null;
  disponibilidad?: number | string | null;
}

export interface VenueResumen {
  id?: number | string | null;
  nombre?: string | null;
  ciudad?: string | null;
  capacidad?: number | string | null;
}

export interface TipoEventoResumen {
  id?: number | string | null;
  nombre?: string | null;
}

export interface EstadoEventoResumen {
  id?: number | string | null;
  nombre?: string | null;
}

export interface EventosFiltros {
  titulo?: string;
  tipoEventoId?: number | string;
  venueId?: number | string;
  estadoEventoId?: number | string;
  fecha?: string;
  fechaDesde?: string;
  fechaHasta?: string;
}
