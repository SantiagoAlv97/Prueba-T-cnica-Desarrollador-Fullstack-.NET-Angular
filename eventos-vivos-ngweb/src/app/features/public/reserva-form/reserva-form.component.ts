import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { EventosService } from '../../../core/services/eventos.service';
import { ReservasService } from '../../../core/services/reservas.service';
import { ToastService } from '../../../core/services/toast.service';
import { Evento } from '../../../core/models/evento.model';

@Component({
  selector: 'app-reserva-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './reserva-form.component.html',
  styleUrl: './reserva-form.component.scss',
})
export class ReservaFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly eventosService = inject(EventosService);
  private readonly reservasService = inject(ReservasService);
  private readonly toastService = inject(ToastService);

  protected readonly evento = signal<Evento | null>(null);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal('');
  protected readonly form = this.fb.nonNullable.group({
    cantidad: [1, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    this.route.paramMap
      .pipe(switchMap((params) => this.eventosService.obtenerPorId(params.get('id') ?? '')))
      .subscribe({
        next: (evento) => {
          this.evento.set(evento);
          this.cargando.set(false);
        },
        error: () => {
          this.error.set('No fue posible cargar el evento para reservar.');
          this.cargando.set(false);
        },
      });
  }

  protected reservar(): void {
    if (this.form.invalid || this.guardando()) {
      this.form.markAllAsTouched();
      return;
    }

    const usuario = this.authService.getUsuario();
    const evento = this.evento();
    const usuarioId = typeof usuario?.usuarioId === 'number' ? usuario.usuarioId : Number(usuario?.usuarioId);
    const eventoId = Number(evento?.eventoId ?? evento?.id);

    if (!usuario || !Number.isFinite(usuarioId) || !evento || !Number.isFinite(eventoId)) {
      this.toastService.error('No fue posible preparar la reserva.');
      return;
    }

    this.guardando.set(true);

    this.reservasService
      .crear({
        eventoId,
        usuarioId,
        cantidad: this.form.getRawValue().cantidad,
      })
      .subscribe({
        next: async () => {
          this.guardando.set(false);
          this.toastService.success('Reserva creada correctamente.');
          await this.router.navigateByUrl('/mis-reservas');
        },
        error: () => {
          this.guardando.set(false);
        },
      });
  }

  protected obtenerPrecio(): string {
    const evento = this.evento();
    const precio = evento?.precioEntrada;

    if (precio === undefined || precio === null || precio === '') {
      return 'Precio por confirmar';
    }

    const numero = Number(precio);

    if (!Number.isFinite(numero)) {
      return 'Precio por confirmar';
    }

    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      maximumFractionDigits: 0,
    }).format(numero);
  }
}
