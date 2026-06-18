import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Reserva } from '../../../core/models/reserva.model';
import { AuthService } from '../../../core/services/auth.service';
import { ReservasService } from '../../../core/services/reservas.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-mis-reservas',
  imports: [CommonModule, RouterLink],
  templateUrl: './mis-reservas.component.html',
  styleUrl: './mis-reservas.component.scss',
})
export class MisReservasComponent {
  private readonly authService = inject(AuthService);
  private readonly reservasService = inject(ReservasService);
  private readonly toastService = inject(ToastService);

  protected readonly reservas = signal<Reserva[]>([]);
  protected readonly cargando = signal(true);
  protected readonly procesandoId = signal<number | string | null>(null);
  protected readonly error = signal('');
  protected readonly totalReservas = computed(() => this.reservas().length);

  constructor() {
    const usuario = this.authService.getUsuario();
    const usuarioId = Number(usuario?.usuarioId);

    if (!Number.isFinite(usuarioId)) {
      this.error.set('No fue posible identificar el usuario autenticado.');
      this.cargando.set(false);
      return;
    }

    this.reservasService.listarPorUsuario(usuarioId).subscribe({
      next: (reservas) => {
        this.reservas.set(reservas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar tus reservas.');
        this.cargando.set(false);
      },
    });
  }

  protected cancelar(reserva: Reserva): void {
    const reservaId = reserva.reservaId ?? reserva.id;

    this.procesandoId.set(reservaId);

    this.reservasService.cancelar(reservaId).subscribe({
      next: () => {
        this.procesandoId.set(null);
        this.reservas.update((reservas) =>
          reservas.map((item) =>
            item.id === reserva.id
              ? {
                  ...item,
                  estadoReserva: 'cancelada',
                  fechaCancelacion: new Date().toISOString(),
                }
              : item,
          ),
        );
        this.toastService.success('Reserva cancelada correctamente.');
      },
      error: () => {
        this.procesandoId.set(null);
      },
    });
  }

  protected obtenerEvento(reserva: Reserva): string {
    if (typeof reserva.evento === 'string' && reserva.evento.trim()) {
      return reserva.evento.trim();
    }

    if (reserva.evento && typeof reserva.evento !== 'string' && reserva.evento.titulo?.trim()) {
      return reserva.evento.titulo.trim();
    }

    return reserva.eventoTitulo?.trim() || 'Evento';
  }

  protected obtenerCodigo(reserva: Reserva): string {
    return reserva.codigoReserva?.trim() || reserva.codigo?.trim() || 'Sin código';
  }

  protected obtenerEstado(reserva: Reserva): string {
    return reserva.estadoReserva?.trim() || 'Pendiente';
  }

  protected puedeCancelar(reserva: Reserva): boolean {
    return !this.obtenerEstado(reserva).toLowerCase().includes('cancel');
  }
}
