import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { TipoEvento } from '../models/tipo-evento.model';

@Injectable({ providedIn: 'root' })
export class TiposEventoService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Eventos/tipos`;

  listar(): Observable<TipoEvento[]> {
    return this.http.get<TipoEvento[]>(this.endpoint);
  }
}
