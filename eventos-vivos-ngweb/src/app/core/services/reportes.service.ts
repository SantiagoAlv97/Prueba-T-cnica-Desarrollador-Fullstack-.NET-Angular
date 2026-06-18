import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ReporteOcupacion } from '../models/reporte.model';

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/Reportes/eventos`;

  obtenerOcupacion(eventoId: number | string): Observable<ReporteOcupacion> {
    return this.http.get<ReporteOcupacion>(`${this.endpoint}/${eventoId}/ocupacion`);
  }
}
