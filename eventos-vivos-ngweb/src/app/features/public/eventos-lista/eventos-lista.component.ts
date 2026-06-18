import { CommonModule, NgOptimizedImage } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, of } from 'rxjs';

import { Evento, EventosFiltros } from '../../../core/models/evento.model';
import { TipoEvento } from '../../../core/models/tipo-evento.model';
import { Venue } from '../../../core/models/venue.model';
import { EventosService } from '../../../core/services/eventos.service';
import { TiposEventoService } from '../../../core/services/tipos-evento.service';
import { VenuesService } from '../../../core/services/venues.service';

@Component({
  selector: 'app-eventos-lista',
  imports: [CommonModule, NgOptimizedImage, RouterLink],
  templateUrl: './eventos-lista.component.html',
  styleUrl: './eventos-lista.component.scss',
})
export class EventosListaComponent implements OnInit {
  private readonly eventosService = inject(EventosService);
  private readonly tiposEventoService = inject(TiposEventoService);
  private readonly venuesService = inject(VenuesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly carouselImages = [
    '/carrusel/1.avif',
    '/carrusel/2.avif',
    '/carrusel/3.avif',
    '/carrusel/4.avif',
  ];

  protected readonly eventos = signal<Evento[]>([]);
  protected readonly tiposEvento = signal<TipoEvento[]>([]);
  protected readonly venues = signal<Venue[]>([]);
  protected readonly carouselIndex = signal(0);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly errorCatalogos = signal('');
  protected readonly titulo = signal('');
  protected readonly tipoEventoId = signal('');
  protected readonly venueId = signal('');
  protected readonly fecha = signal('');
  protected readonly resultadosTexto = computed(() => {
    const total = this.eventos().length;
    return `${total} ${total === 1 ? 'evento encontrado' : 'eventos encontrados'}`;
  });
  protected readonly hayFiltrosActivos = computed(
    () =>
      Boolean(
        this.titulo().trim() || this.tipoEventoId().trim() || this.venueId().trim() || this.fecha(),
      ),
  );

  ngOnInit(): void {
    this.iniciarCarousel();
    this.cargarCatalogos();
    this.cargarEventos();
  }

  protected aplicarFiltros(event: Event): void {
    event.preventDefault();
    this.cargarEventos(this.obtenerFiltrosActivos());
  }

  protected limpiarFiltros(): void {
    this.titulo.set('');
    this.tipoEventoId.set('');
    this.venueId.set('');
    this.fecha.set('');
    this.cargarEventos();
  }

  protected reintentar(): void {
    this.cargarCatalogos();
    this.cargarEventos(this.obtenerFiltrosActivos());
  }

  protected onTituloChange(event: Event): void {
    this.titulo.set(this.obtenerValor(event));
  }

  protected onTipoEventoChange(event: Event): void {
    this.tipoEventoId.set(this.obtenerValor(event));
  }

  protected onVenueChange(event: Event): void {
    this.venueId.set(this.obtenerValor(event));
  }

  protected onFechaChange(event: Event): void {
    this.fecha.set(this.obtenerValor(event));
  }

  protected trackByEvento(_: number, evento: Evento): number | string {
    return evento.id;
  }

  protected obtenerDescripcionCorta(evento: Evento): string {
    const descripcion = evento.descripcion?.trim();

    if (!descripcion) {
      return 'Sin descripción disponible.';
    }

    return descripcion.length > 170 ? `${descripcion.slice(0, 167)}...` : descripcion;
  }

  protected obtenerVenue(evento: Evento): string {
    if (typeof evento.venue === 'string' && evento.venue.trim()) {
      return evento.venue.trim();
    }

    if (evento.venue && typeof evento.venue !== 'string' && evento.venue.nombre?.trim()) {
      return evento.venue.nombre.trim();
    }

    if (evento.venueNombre?.trim()) {
      return evento.venueNombre.trim();
    }

    if (evento.venueId !== undefined && evento.venueId !== null && evento.venueId !== '') {
      return `Venue ${evento.venueId}`;
    }

    return '';
  }

  protected obtenerCiudad(evento: Evento): string {
    if (evento.ciudad?.trim()) {
      return evento.ciudad.trim();
    }

    if (evento.venue && typeof evento.venue !== 'string' && evento.venue.ciudad?.trim()) {
      return evento.venue.ciudad.trim();
    }

    return '';
  }

  protected obtenerTipoEvento(evento: Evento): string {
    if (typeof evento.tipoEvento === 'string' && evento.tipoEvento.trim()) {
      return evento.tipoEvento.trim();
    }

    if (
      evento.tipoEvento &&
      typeof evento.tipoEvento !== 'string' &&
      evento.tipoEvento.nombre?.trim()
    ) {
      return evento.tipoEvento.nombre.trim();
    }

    if (evento.tipoEventoNombre?.trim()) {
      return evento.tipoEventoNombre.trim();
    }

    if (
      evento.tipoEventoId !== undefined &&
      evento.tipoEventoId !== null &&
      evento.tipoEventoId !== ''
    ) {
      return `Tipo ${evento.tipoEventoId}`;
    }

    return '';
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

    if (evento.estadoEventoNombre?.trim()) {
      return evento.estadoEventoNombre.trim();
    }

    if (
      evento.estadoEventoId !== undefined &&
      evento.estadoEventoId !== null &&
      evento.estadoEventoId !== ''
    ) {
      return `Estado ${evento.estadoEventoId}`;
    }

    return '';
  }

  protected obtenerCapacidad(evento: Evento): string {
    const disponibles = this.normalizarNumero(evento.cuposDisponibles ?? evento.disponibilidad);

    if (disponibles !== null) {
      return `${disponibles} disponibles`;
    }

    const capacidadMaxima = this.normalizarNumero(evento.capacidadMaxima);

    if (capacidadMaxima !== null) {
      return `${capacidadMaxima} cupos`;
    }

    if (evento.venue && typeof evento.venue !== 'string') {
      const capacidadVenue = this.normalizarNumero(evento.venue.capacidad);

      if (capacidadVenue !== null) {
        return `${capacidadVenue} cupos`;
      }
    }

    return '';
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

  protected obtenerVenueLabel(venue: Venue): string {
    return venue.ciudad ? `${venue.nombre} - ${venue.ciudad}` : venue.nombre;
  }

  private iniciarCarousel(): void {
    if (this.carouselImages.length < 2) {
      return;
    }

    const intervalId = window.setInterval(() => {
      this.carouselIndex.update((index) => (index + 1) % this.carouselImages.length);
    }, 4000);

    this.destroyRef.onDestroy(() => {
      window.clearInterval(intervalId);
    });
  }

  private cargarEventos(filtros?: EventosFiltros): void {
    this.cargando.set(true);
    this.error.set('');

    this.eventosService.listar(filtros).subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar los eventos en este momento.');
        this.eventos.set([]);
        this.cargando.set(false);
      },
    });
  }

  private cargarCatalogos(): void {
    this.errorCatalogos.set('');

    let falloTipos = false;
    let falloVenues = false;

    forkJoin({
      tiposEvento: this.tiposEventoService.listar().pipe(
        catchError(() => {
          falloTipos = true;
          return of([] as TipoEvento[]);
        }),
      ),
      venues: this.venuesService.listar().pipe(
        catchError(() => {
          falloVenues = true;
          return of([] as Venue[]);
        }),
      ),
    }).subscribe(({ tiposEvento, venues }) => {
      this.tiposEvento.set(tiposEvento);
      this.venues.set(venues);

      if (falloTipos || falloVenues) {
        this.errorCatalogos.set('No fue posible cargar uno o más filtros desde la API.');
      }
    });
  }

  private obtenerFiltrosActivos(): EventosFiltros {
    const filtros: EventosFiltros = {};

    if (this.titulo().trim()) {
      filtros.titulo = this.titulo().trim();
    }

    if (this.tipoEventoId()) {
      filtros.tipoEventoId = this.tipoEventoId();
    }

    if (this.venueId()) {
      filtros.venueId = this.venueId();
    }

    if (this.fecha()) {
      filtros.fecha = this.fecha();
    }

    return filtros;
  }

  private obtenerValor(event: Event): string {
    return (event.target as HTMLInputElement | HTMLSelectElement).value;
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
