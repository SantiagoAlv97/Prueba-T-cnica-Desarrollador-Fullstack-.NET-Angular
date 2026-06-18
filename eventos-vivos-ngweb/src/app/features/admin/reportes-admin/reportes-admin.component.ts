import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { Evento } from '../../../core/models/evento.model';
import { ReporteOcupacion } from '../../../core/models/reporte.model';
import { EventosService } from '../../../core/services/eventos.service';
import { ReportesService } from '../../../core/services/reportes.service';

@Component({
  selector: 'app-reportes-admin',
  imports: [CommonModule],
  templateUrl: './reportes-admin.component.html',
  styleUrl: './reportes-admin.component.scss',
})
export class ReportesAdminComponent {
  private readonly eventosService = inject(EventosService);
  private readonly reportesService = inject(ReportesService);

  protected readonly eventos = signal<Evento[]>([]);
  protected readonly reporte = signal<ReporteOcupacion | null>(null);
  protected readonly eventoSeleccionado = signal('');
  protected readonly cargandoEventos = signal(true);
  protected readonly cargandoReporte = signal(false);
  protected readonly error = signal('');

  constructor() {
    this.eventosService.listar().subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.cargandoEventos.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar los eventos para reportes.');
        this.cargandoEventos.set(false);
      },
    });
  }

  protected seleccionarEvento(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.eventoSeleccionado.set(value);

    if (!value) {
      this.reporte.set(null);
      return;
    }

    this.cargandoReporte.set(true);
    this.reportesService.obtenerOcupacion(value).subscribe({
      next: (reporte) => {
        this.reporte.set(reporte);
        this.cargandoReporte.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar el reporte de ocupación.');
        this.cargandoReporte.set(false);
      },
    });
  }
}
