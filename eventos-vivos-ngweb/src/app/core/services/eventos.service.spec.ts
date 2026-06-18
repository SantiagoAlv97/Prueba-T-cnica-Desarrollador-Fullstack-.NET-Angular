import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';

import { EventosService } from './eventos.service';

describe('EventosService', () => {
  let service: EventosService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), EventosService],
    });

    service = TestBed.inject(EventosService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should list events with mapped filters', () => {
    let response: unknown;

    service
      .listar({
        titulo: 'Tech',
        tipoEventoId: 2,
        venueId: 3,
        estadoEventoId: 1,
        fecha: '2026-08-15',
      })
      .subscribe((value) => {
        response = value;
      });

    const request = httpMock.expectOne((req) => req.url === '/api/Eventos');

    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('Titulo')).toBe('Tech');
    expect(request.request.params.get('TipoEventoId')).toBe('2');
    expect(request.request.params.get('VenueId')).toBe('3');
    expect(request.request.params.get('EstadoEventoId')).toBe('1');
    expect(request.request.params.get('FechaDesde')).toBe('2026-08-15T00:00:00');
    expect(request.request.params.get('FechaHasta')).toBe('2026-08-15T23:59:59');

    request.flush([
      {
        eventoId: 10,
        titulo: 'Conferencia Angular',
      },
    ]);

    expect(response).toEqual([
      {
        eventoId: 10,
        id: 10,
        titulo: 'Conferencia Angular',
      },
    ]);
  });

  it('should get an event by id', () => {
    let response: unknown;

    service.obtenerPorId(5).subscribe((value) => {
      response = value;
    });

    const request = httpMock.expectOne('/api/Eventos/5');
    expect(request.request.method).toBe('GET');

    request.flush({
      eventoId: 5,
      titulo: 'Evento detalle',
    });

    expect(response).toEqual({
      eventoId: 5,
      id: 5,
      titulo: 'Evento detalle',
    });
  });

  it('should create an event', () => {
    let response: unknown;
    const payload = {
      titulo: 'Nuevo evento',
      descripcion: 'Descripcion',
      venueId: 1,
      tipoEventoId: 2,
      capacidadMaxima: 100,
      fechaInicio: '2026-08-15T18:00:00',
      fechaFin: '2026-08-15T20:00:00',
      precioEntrada: 50000,
      estadoEventoId: 1,
    };

    service.crear(payload).subscribe((value) => {
      response = value;
    });

    const request = httpMock.expectOne('/api/Eventos');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);

    request.flush({
      eventoId: 12,
      titulo: 'Nuevo evento',
    });

    expect(response).toEqual({
      eventoId: 12,
      id: 12,
      titulo: 'Nuevo evento',
    });
  });

  it('should update an event', () => {
    const payload = {
      titulo: 'Evento editado',
      descripcion: 'Descripcion',
      venueId: 1,
      tipoEventoId: 2,
      capacidadMaxima: 100,
      fechaInicio: '2026-08-15T18:00:00',
      fechaFin: '2026-08-15T20:00:00',
      precioEntrada: 50000,
      estadoEventoId: 2,
    };

    service.actualizar(7, payload).subscribe();

    const request = httpMock.expectOne('/api/Eventos/7');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(payload);

    request.flush({
      eventoId: 7,
      titulo: 'Evento editado',
    });
  });

  it('should cancel an event', () => {
    service.cancelar(9).subscribe();

    const request = httpMock.expectOne('/api/Eventos/9/cancelar');
    expect(request.request.method).toBe('PATCH');
    expect(request.request.body).toEqual({});

    request.flush({});
  });
});
