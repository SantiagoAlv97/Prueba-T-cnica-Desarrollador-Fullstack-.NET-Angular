export interface Reserva {
  id: number | string;
  reservaId?: number | string;
  codigoReserva?: string | null;
  codigo?: string | null;
  eventoId?: number | string | null;
  eventoTitulo?: string | null;
  evento?: string | EventoReservaResumen | null;
  usuarioId?: number | string | null;
  usuarioNombre?: string | null;
  usuarioEmail?: string | null;
  cantidad: number;
  estadoReservaId?: number | string | null;
  estadoReserva?: string | null;
  fechaReserva?: string | null;
  fechaPago?: string | null;
  fechaCancelacion?: string | null;
  totalPagado?: number | string | null;
  precioTotal?: number | string | null;
}

export interface EventoReservaResumen {
  eventoId?: number | string | null;
  titulo?: string | null;
}

export interface CrearReservaRequest {
  eventoId: number;
  cantidad: number;
}
