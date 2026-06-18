import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { EstadoEvento } from '../models/estado-evento.model';

@Injectable({ providedIn: 'root' })
export class EstadosEventoService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Eventos/estados`;

  listar(): Observable<EstadoEvento[]> {
    return this.http.get<EstadoEvento[]>(this.endpoint);
  }
}
