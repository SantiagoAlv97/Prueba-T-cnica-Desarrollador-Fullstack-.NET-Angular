import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { forkJoin } from 'rxjs';

import { EventoFormComponent } from '../evento-form/evento-form.component';
import { Evento } from '../../../core/models/evento.model';
import { EstadoEvento } from '../../../core/models/estado-evento.model';
import { TipoEvento } from '../../../core/models/tipo-evento.model';
import { Venue } from '../../../core/models/venue.model';
import { EstadosEventoService } from '../../../core/services/estados-evento.service';
import { EventosService, EventoPayload } from '../../../core/services/eventos.service';
import { TiposEventoService } from '../../../core/services/tipos-evento.service';
import { ToastService } from '../../../core/services/toast.service';
import { VenuesService } from '../../../core/services/venues.service';

@Component({
  selector: 'app-eventos-admin',
  imports: [CommonModule, EventoFormComponent],
  templateUrl: './eventos-admin.component.html',
  styleUrl: './eventos-admin.component.scss',
})
export class EventosAdminComponent {
  private readonly eventosService = inject(EventosService);
  private readonly tiposEventoService = inject(TiposEventoService);
  private readonly estadosEventoService = inject(EstadosEventoService);
  private readonly venuesService = inject(VenuesService);
  private readonly toastService = inject(ToastService);

  protected readonly eventos = signal<Evento[]>([]);
  protected readonly tiposEvento = signal<TipoEvento[]>([]);
  protected readonly estadosEvento = signal<EstadoEvento[]>([]);
  protected readonly venues = signal<Venue[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly guardando = signal(false);
  protected readonly eventoEditando = signal<Evento | null>(null);
  protected readonly formularioAbierto = signal(false);

  constructor() {
    this.recargar();
  }

  protected abrirCrear(): void {
    this.eventoEditando.set(null);
    this.formularioAbierto.set(true);
  }

  protected abrirEditar(evento: Evento): void {
    this.eventoEditando.set(evento);
    this.formularioAbierto.set(true);
  }

  protected cerrarFormulario(): void {
    this.formularioAbierto.set(false);
    this.eventoEditando.set(null);
  }

  protected guardar(payload: EventoPayload): void {
    this.guardando.set(true);
    const evento = this.eventoEditando();

    const request = evento
      ? this.eventosService.actualizar(evento.eventoId ?? evento.id, payload)
      : this.eventosService.crear(payload);

    request.subscribe({
      next: () => {
        this.guardando.set(false);
        this.toastService.success(
          evento ? 'Evento actualizado correctamente.' : 'Evento creado correctamente.',
        );
        this.cerrarFormulario();
        this.recargar();
      },
      error: () => {
        this.guardando.set(false);
      },
    });
  }

  protected cancelarEvento(evento: Evento): void {
    this.eventosService.cancelar(evento.eventoId ?? evento.id).subscribe({
      next: () => {
        this.toastService.success('Evento cancelado correctamente.');
        this.recargar();
      },
      error: () => {},
    });
  }

  protected obtenerVenue(evento: Evento): string {
    return evento.venueNombre?.trim() || '';
  }

  protected obtenerTipo(evento: Evento): string {
    if (typeof evento.tipoEvento === 'string' && evento.tipoEvento.trim()) {
      return evento.tipoEvento.trim();
    }

    return evento.tipoEventoNombre?.trim() || '';
  }

  private recargar(): void {
    this.cargando.set(true);
    this.error.set('');

    forkJoin({
      eventos: this.eventosService.listar(),
      tiposEvento: this.tiposEventoService.listar(),
      estadosEvento: this.estadosEventoService.listar(),
      venues: this.venuesService.listar(),
    }).subscribe({
      next: ({ eventos, tiposEvento, estadosEvento, venues }) => {
        this.eventos.set(eventos);
        this.tiposEvento.set(tiposEvento);
        this.estadosEvento.set(estadosEvento);
        this.venues.set(venues);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar la administración de eventos.');
        this.cargando.set(false);
      },
    });
  }
}
