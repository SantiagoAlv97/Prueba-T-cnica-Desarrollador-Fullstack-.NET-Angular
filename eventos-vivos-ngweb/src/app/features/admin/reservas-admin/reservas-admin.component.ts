import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { Reserva } from '../../../core/models/reserva.model';
import { ReservasService } from '../../../core/services/reservas.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-reservas-admin',
  imports: [CommonModule],
  templateUrl: './reservas-admin.component.html',
  styleUrl: './reservas-admin.component.scss',
})
export class ReservasAdminComponent {
  private static readonly estadoPendientePagoId = 1;
  private static readonly estadoConfirmadaId = 2;
  private static readonly estadoCanceladaId = 3;
  private static readonly estadoPerdidaId = 4;

  private readonly reservasService = inject(ReservasService);
  private readonly toastService = inject(ToastService);

  protected readonly reservas = signal<Reserva[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');

  constructor() {
    this.recargar();
  }

  protected confirmarPago(reserva: Reserva): void {
    this.reservasService.confirmarPago(reserva.reservaId ?? reserva.id).subscribe({
      next: () => {
        this.toastService.success('Pago confirmado correctamente.');
        this.recargar();
      },
      error: () => {},
    });
  }

  protected cancelar(reserva: Reserva): void {
    this.reservasService.cancelar(reserva.reservaId ?? reserva.id).subscribe({
      next: () => {
        this.toastService.success('Reserva cancelada correctamente.');
        this.recargar();
      },
      error: () => {},
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

  protected obtenerUsuario(reserva: Reserva): string {
    return reserva.usuarioNombre?.trim() || reserva.usuarioEmail?.trim() || 'Usuario';
  }

  protected puedeConfirmarPago(reserva: Reserva): boolean {
    const estadoId = this.normalizarEstadoId(reserva);

    if (estadoId !== null) {
      return estadoId === ReservasAdminComponent.estadoPendientePagoId;
    }

    return this.normalizarEstadoTexto(reserva) === 'pendiente_pago';
  }

  protected puedeCancelar(reserva: Reserva): boolean {
    const estadoId = this.normalizarEstadoId(reserva);

    if (estadoId !== null) {
      return (
        estadoId === ReservasAdminComponent.estadoPendientePagoId ||
        estadoId === ReservasAdminComponent.estadoConfirmadaId
      );
    }

    const estado = this.normalizarEstadoTexto(reserva);
    return estado === 'pendiente_pago' || estado === 'confirmada';
  }

  protected tieneAcciones(reserva: Reserva): boolean {
    return this.puedeConfirmarPago(reserva) || this.puedeCancelar(reserva);
  }

  private recargar(): void {
    this.cargando.set(true);
    this.error.set('');

    this.reservasService.listar().subscribe({
      next: (reservas) => {
        this.reservas.set(reservas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar las reservas.');
        this.cargando.set(false);
      },
    });
  }

  private normalizarEstadoId(reserva: Reserva): number | null {
    const estadoId = reserva.estadoReservaId;

    if (typeof estadoId === 'number' && Number.isFinite(estadoId)) {
      return estadoId;
    }

    if (typeof estadoId === 'string' && estadoId.trim()) {
      const numero = Number(estadoId);
      return Number.isFinite(numero) ? numero : null;
    }

    return null;
  }

  private normalizarEstadoTexto(reserva: Reserva): string {
    return (reserva.estadoReserva ?? '')
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '_');
  }
}
