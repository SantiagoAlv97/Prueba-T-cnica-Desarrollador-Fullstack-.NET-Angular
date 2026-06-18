import { CommonModule } from '@angular/common';
import { Component, effect, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Venue } from '../../../core/models/venue.model';
import { VenuePayload } from '../../../core/services/venues.service';

@Component({
  selector: 'app-venue-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './venue-form.component.html',
  styleUrl: './venue-form.component.scss',
})
export class VenueFormComponent {
  private readonly fb = inject(FormBuilder);
  protected readonly nombreMaxLength = 200;
  protected readonly ciudadMaxLength = 50;

  readonly venue = input<Venue | null>(null);
  readonly submitting = input(false);
  readonly submitLabel = input('Guardar venue');
  readonly title = input('Nuevo venue');

  readonly submitted = output<VenuePayload>();
  readonly cancelled = output<void>();

  protected readonly form = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(this.nombreMaxLength)]],
    capacidad: [1, [Validators.required, Validators.min(1)]],
    ciudad: ['', [Validators.required, Validators.maxLength(this.ciudadMaxLength)]],
  });

  constructor() {
    effect(() => {
      const venue = this.venue();

      if (!venue) {
        this.form.reset({
          nombre: '',
          capacidad: 1,
          ciudad: '',
        });
        return;
      }

      this.form.reset({
        nombre: venue.nombre ?? '',
        capacidad: venue.capacidad ?? 1,
        ciudad: venue.ciudad ?? '',
      });
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitted.emit({
      nombre: value.nombre.trim(),
      capacidad: value.capacidad,
      ciudad: value.ciudad.trim(),
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
}
