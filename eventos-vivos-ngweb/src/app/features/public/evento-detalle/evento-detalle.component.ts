import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { EventosService } from '../../../core/services/eventos.service';
import { Evento } from '../../../core/models/evento.model';

@Component({
  selector: 'app-evento-detalle',
  imports: [CommonModule, RouterLink],
  templateUrl: './evento-detalle.component.html',
  styleUrl: './evento-detalle.component.scss',
})
export class EventoDetalleComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly eventosService = inject(EventosService);
  private readonly authService = inject(AuthService);

  protected readonly evento = signal<Evento | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly estaAutenticado = this.authService.estaAutenticado;
  protected readonly eventoId = computed(() => this.evento()?.eventoId ?? this.evento()?.id ?? '');

  constructor() {
    this.route.paramMap
      .pipe(switchMap((params) => this.eventosService.obtenerPorId(params.get('id') ?? '')))
      .subscribe({
        next: (evento) => {
          this.evento.set(evento);
          this.cargando.set(false);
        },
        error: () => {
          this.error.set('No fue posible cargar el detalle del evento.');
          this.cargando.set(false);
        },
      });
  }

  protected obtenerVenue(evento: Evento): string {
    if (typeof evento.venue === 'string' && evento.venue.trim()) {
      return evento.venue.trim();
    }

    if (evento.venue && typeof evento.venue !== 'string' && evento.venue.nombre?.trim()) {
      return evento.venue.nombre.trim();
    }

    return evento.venueNombre?.trim() || '';
  }

  protected obtenerTipo(evento: Evento): string {
    if (typeof evento.tipoEvento === 'string' && evento.tipoEvento.trim()) {
      return evento.tipoEvento.trim();
    }

    if (evento.tipoEvento && typeof evento.tipoEvento !== 'string' && evento.tipoEvento.nombre?.trim()) {
      return evento.tipoEvento.nombre.trim();
    }

    return evento.tipoEventoNombre?.trim() || '';
  }

  protected obtenerEstado(evento: Evento): string {
    if (typeof evento.estadoEvento === 'string' && evento.estadoEvento.trim()) {
      return evento.estadoEvento.trim();
    }

    if (
      evento.estadoEvento &&
      typeof evento.estadoEvento !== 'string' &&
      evento.estadoEvento.nombre?.trim()
    ) {
      return evento.estadoEvento.nombre.trim();
    }

    return evento.estadoEventoNombre?.trim() || '';
  }

  protected obtenerPrecio(evento: Evento): string {
    const precio = this.normalizarNumero(evento.precioEntrada);

    if (precio === null) {
      return 'Precio por confirmar';
    }

    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      maximumFractionDigits: 0,
    }).format(precio);
  }

  protected obtenerCapacidad(evento: Evento): string {
    const disponibles = this.normalizarNumero(evento.cuposDisponibles ?? evento.disponibilidad);

    if (disponibles !== null) {
      return `${disponibles} disponibles`;
    }

    const capacidad = this.normalizarNumero(evento.capacidadMaxima);

    return capacidad !== null ? `${capacidad} cupos` : 'Capacidad por confirmar';
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
