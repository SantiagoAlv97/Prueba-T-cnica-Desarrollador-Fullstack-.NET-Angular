import { CommonModule } from '@angular/common';
import { Component, effect, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Evento } from '../../../core/models/evento.model';
import { EstadoEvento } from '../../../core/models/estado-evento.model';
import { TipoEvento } from '../../../core/models/tipo-evento.model';
import { Venue } from '../../../core/models/venue.model';
import { EventoPayload } from '../../../core/services/eventos.service';
import {
  toBackendLocalDateTime,
  toDateTimeLocalInputValue,
} from '../../../core/utils/date-time.util';

@Component({
  selector: 'app-evento-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './evento-form.component.html',
  styleUrl: './evento-form.component.scss',
})
export class EventoFormComponent {
  private readonly fb = inject(FormBuilder);
  protected readonly tituloMinLength = 5;
  protected readonly tituloMaxLength = 100;
  protected readonly descripcionMinLength = 10;
  protected readonly descripcionMaxLength = 500;

  readonly evento = input<Evento | null>(null);
  readonly tiposEvento = input<TipoEvento[]>([]);
  readonly estadosEvento = input<EstadoEvento[]>([]);
  readonly venues = input<Venue[]>([]);
  readonly submitting = input(false);
  readonly submitLabel = input('Guardar evento');
  readonly title = input('Nuevo evento');

  readonly submitted = output<EventoPayload>();
  readonly cancelled = output<void>();

  protected readonly form = this.fb.nonNullable.group({
    titulo: [
      '',
      [
        Validators.required,
        Validators.minLength(this.tituloMinLength),
        Validators.maxLength(this.tituloMaxLength),
      ],
    ],
    descripcion: [
      '',
      [
        Validators.required,
        Validators.minLength(this.descripcionMinLength),
        Validators.maxLength(this.descripcionMaxLength),
      ],
    ],
    venueId: [0, [Validators.required, Validators.min(1)]],
    tipoEventoId: [0, [Validators.required, Validators.min(1)]],
    capacidadMaxima: [1, [Validators.required, Validators.min(1)]],
    fechaInicio: ['', [Validators.required]],
    fechaFin: ['', [Validators.required]],
    precioEntrada: [0, [Validators.required, Validators.min(1)]],
    estadoEventoId: [1, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    effect(() => {
      const evento = this.evento();

      if (!evento) {
        this.form.reset({
          titulo: '',
          descripcion: '',
          venueId: 0,
          tipoEventoId: 0,
          capacidadMaxima: 1,
          fechaInicio: '',
          fechaFin: '',
          precioEntrada: 0,
          estadoEventoId: 1,
        });
        return;
      }

      this.form.reset({
        titulo: evento.titulo ?? '',
        descripcion: evento.descripcion ?? '',
        venueId: this.normalizarNumero(evento.venueId) ?? 0,
        tipoEventoId: this.normalizarNumero(evento.tipoEventoId) ?? 0,
        capacidadMaxima: this.normalizarNumero(evento.capacidadMaxima) ?? 1,
        fechaInicio: toDateTimeLocalInputValue(evento.fechaInicio),
        fechaFin: toDateTimeLocalInputValue(evento.fechaFin),
        precioEntrada: this.normalizarNumero(evento.precioEntrada) ?? 0,
        estadoEventoId: this.normalizarNumero(evento.estadoEventoId) ?? 1,
      });
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const fechaInicio = toBackendLocalDateTime(value.fechaInicio);
    const fechaFin = toBackendLocalDateTime(value.fechaFin);

    if (new Date(value.fechaFin) < new Date(value.fechaInicio)) {
      this.form.controls.fechaFin.setErrors({ beforeStart: true });
      this.form.controls.fechaFin.markAsTouched();
      return;
    }

    if (!fechaInicio || !fechaFin) {
      this.form.controls.fechaInicio.setErrors({ invalidDateTime: true });
      this.form.controls.fechaFin.setErrors({ invalidDateTime: true });
      this.form.markAllAsTouched();
      return;
    }

    this.submitted.emit({
      titulo: value.titulo.trim(),
      descripcion: value.descripcion.trim(),
      venueId: value.venueId,
      tipoEventoId: value.tipoEventoId,
      capacidadMaxima: value.capacidadMaxima,
      fechaInicio,
      fechaFin,
      precioEntrada: value.precioEntrada,
      estadoEventoId: value.estadoEventoId,
    });
  }

  protected cancel(): void {
    this.cancelled.emit();
  }

  protected shouldShowError(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.touched || control.dirty);
  }

  protected hasError(controlName: keyof typeof this.form.controls, errorCode: string): boolean {
    return Boolean(this.form.controls[controlName].errors?.[errorCode]);
  }

  private normalizarNumero(value: number | string | null | undefined): number | null {
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value;
    }

    if (typeof value === 'string' && value.trim()) {
      const numero = Number(value);
      return Number.isFinite(numero) ? numero : null;
    }

    return null;
  }
}
