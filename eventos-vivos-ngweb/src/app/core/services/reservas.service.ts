import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CrearReservaRequest, Reserva } from '../models/reserva.model';

type ReservaApi = Partial<Reserva> & {
  reservaId?: number | string;
};

@Injectable({ providedIn: 'root' })
export class ReservasService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Reservas`;

  listar(): Observable<Reserva[]> {
    return this.http.get<ReservaApi[]>(this.endpoint).pipe(
      map((reservas) => reservas.map((reserva) => this.normalizarReserva(reserva))),
    );
  }

  obtenerPorId(id: number | string): Observable<Reserva> {
    return this.http.get<ReservaApi>(`${this.endpoint}/${id}`).pipe(
      map((reserva) => this.normalizarReserva(reserva)),
    );
  }

  listarMisReservas(): Observable<Reserva[]> {
    return this.http.get<ReservaApi[]>(`${this.endpoint}/mis-reservas`).pipe(
      map((reservas) => reservas.map((reserva) => this.normalizarReserva(reserva))),
    );
  }

  listarPorUsuario(usuarioId: number | string): Observable<Reserva[]> {
    return this.http.get<ReservaApi[]>(`${this.endpoint}/usuario/${usuarioId}`).pipe(
      map((reservas) => reservas.map((reserva) => this.normalizarReserva(reserva))),
    );
  }

  crear(payload: CrearReservaRequest): Observable<Reserva> {
    return this.http.post<ReservaApi>(this.endpoint, payload).pipe(
      map((reserva) => this.normalizarReserva(reserva)),
    );
  }

  confirmarPago(id: number | string): Observable<void> {
    return this.http.patch<void>(`${this.endpoint}/${id}/confirmar-pago`, {});
  }

  cancelar(id: number | string): Observable<void> {
    return this.http.patch<void>(`${this.endpoint}/${id}/cancelar`, {});
  }

  private normalizarReserva(reserva: ReservaApi): Reserva {
    const reservaId = reserva.reservaId ?? reserva.id ?? `${reserva.eventoId ?? 'reserva'}`;

    return {
      ...reserva,
      id: reserva.id ?? reservaId,
      reservaId,
      cantidad: this.normalizarNumero(reserva.cantidad) ?? 0,
    };
  }

  private normalizarNumero(valor: number | string | null | undefined): number | null {
    if (typeof valor === 'number' && Number.isFinite(valor)) {
      return valor;
    }

    if (typeof valor === 'string' && valor.trim()) {
      const numero = Number(valor);
      return Number.isFinite(numero) ? numero : null;
    }

    return null;
  }
}
