import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Venue } from '../models/venue.model';

export interface VenuePayload {
  nombre: string;
  capacidad: number;
  ciudad: string;
}

@Injectable({ providedIn: 'root' })
export class VenuesService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Venues`;

  listar(): Observable<Venue[]> {
    return this.http.get<Venue[]>(this.endpoint);
  }

  obtenerPorId(id: number | string): Observable<Venue> {
    return this.http.get<Venue>(`${this.endpoint}/${id}`);
  }

  crear(payload: VenuePayload): Observable<Venue> {
    return this.http.post<Venue>(this.endpoint, payload);
  }

  actualizar(id: number | string, payload: VenuePayload): Observable<Venue> {
    return this.http.put<Venue>(`${this.endpoint}/${id}`, payload);
  }

  eliminar(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
