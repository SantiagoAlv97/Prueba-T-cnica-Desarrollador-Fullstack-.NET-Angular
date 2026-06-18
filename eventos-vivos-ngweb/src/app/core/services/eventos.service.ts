import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Evento, EventosFiltros } from '../models/evento.model';

type EventoApi = Partial<Evento> & {
  eventoId?: number | string;
};

export interface EventoPayload {
  titulo: string;
  descripcion: string;
  venueId: number;
  tipoEventoId: number;
  capacidadMaxima: number;
  fechaInicio: string;
  fechaFin: string;
  precioEntrada: number;
  estadoEventoId?: number;
}

@Injectable({ providedIn: 'root' })
export class EventosService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Eventos`;

  listar(filtros?: EventosFiltros): Observable<Evento[]> {
    let params = new HttpParams();

    if (filtros?.titulo?.trim()) {
      params = params.set('Titulo', filtros.titulo.trim());
    }

    if (filtros?.tipoEventoId !== undefined && filtros.tipoEventoId !== '') {
      params = params.set('TipoEventoId', String(filtros.tipoEventoId));
    }

    if (filtros?.venueId !== undefined && filtros.venueId !== '') {
      params = params.set('VenueId', String(filtros.venueId));
    }

    if (filtros?.estadoEventoId !== undefined && filtros.estadoEventoId !== '') {
      params = params.set('EstadoEventoId', String(filtros.estadoEventoId));
    }

    if (filtros?.fechaDesde) {
      params = params.set('FechaDesde', filtros.fechaDesde);
    }

    if (filtros?.fechaHasta) {
      params = params.set('FechaHasta', filtros.fechaHasta);
    }

    if (filtros?.fecha && !filtros.fechaDesde && !filtros.fechaHasta) {
      params = params
        .set('FechaDesde', `${filtros.fecha}T00:00:00`)
        .set('FechaHasta', `${filtros.fecha}T23:59:59`);
    }

    return this.http.get<EventoApi[]>(this.endpoint, { params }).pipe(
      map((eventos) => eventos.map((evento) => this.normalizarEvento(evento))),
    );
  }

  obtenerPorId(id: number | string): Observable<Evento> {
    return this.http.get<EventoApi>(`${this.endpoint}/${id}`).pipe(
      map((evento) => this.normalizarEvento(evento)),
    );
  }

  crear(payload: EventoPayload): Observable<Evento> {
    return this.http.post<EventoApi>(this.endpoint, payload).pipe(
      map((evento) => this.normalizarEvento(evento)),
    );
  }

  actualizar(id: number | string, payload: EventoPayload): Observable<Evento> {
    return this.http.put<EventoApi>(`${this.endpoint}/${id}`, payload).pipe(
      map((evento) => this.normalizarEvento(evento)),
    );
  }

  cancelar(id: number | string): Observable<void> {
    return this.http.patch<void>(`${this.endpoint}/${id}/cancelar`, {});
  }

  private normalizarEvento(evento: EventoApi): Evento {
    const eventoId = evento.eventoId ?? evento.id ?? `${evento.titulo ?? 'evento'}`;

    return {
      ...evento,
      id: evento.id ?? eventoId,
      eventoId,
      titulo: evento.titulo ?? 'Evento sin título',
    };
  }
}
