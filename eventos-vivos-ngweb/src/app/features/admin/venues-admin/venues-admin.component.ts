import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { VenueFormComponent } from '../venue-form/venue-form.component';
import { Venue } from '../../../core/models/venue.model';
import { ToastService } from '../../../core/services/toast.service';
import { VenuePayload, VenuesService } from '../../../core/services/venues.service';

@Component({
  selector: 'app-venues-admin',
  imports: [CommonModule, VenueFormComponent],
  templateUrl: './venues-admin.component.html',
  styleUrl: './venues-admin.component.scss',
})
export class VenuesAdminComponent {
  private readonly venuesService = inject(VenuesService);
  private readonly toastService = inject(ToastService);

  protected readonly venues = signal<Venue[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly guardando = signal(false);
  protected readonly venueEditando = signal<Venue | null>(null);
  protected readonly formularioAbierto = signal(false);

  constructor() {
    this.recargar();
  }

  protected abrirCrear(): void {
    this.venueEditando.set(null);
    this.formularioAbierto.set(true);
  }

  protected abrirEditar(venue: Venue): void {
    this.venueEditando.set(venue);
    this.formularioAbierto.set(true);
  }

  protected cerrarFormulario(): void {
    this.formularioAbierto.set(false);
    this.venueEditando.set(null);
  }

  protected guardar(payload: VenuePayload): void {
    this.guardando.set(true);
    const venue = this.venueEditando();
    const request = venue
      ? this.venuesService.actualizar(venue.venueId, payload)
      : this.venuesService.crear(payload);

    request.subscribe({
      next: () => {
        this.guardando.set(false);
        this.toastService.success(
          venue ? 'Venue actualizado correctamente.' : 'Venue creado correctamente.',
        );
        this.cerrarFormulario();
        this.recargar();
      },
      error: () => {
        this.guardando.set(false);
      },
    });
  }

  protected eliminar(venue: Venue): void {
    this.venuesService.eliminar(venue.venueId).subscribe({
      next: () => {
        this.toastService.success('Venue eliminado correctamente.');
        this.recargar();
      },
      error: () => {},
    });
  }

  private recargar(): void {
    this.cargando.set(true);
    this.error.set('');

    this.venuesService.listar().subscribe({
      next: (venues) => {
        this.venues.set(venues);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar los venues.');
        this.cargando.set(false);
      },
    });
  }
}
