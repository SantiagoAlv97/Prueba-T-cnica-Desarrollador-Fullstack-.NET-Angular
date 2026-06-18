import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';

import { ReservasService } from './reservas.service';

describe('ReservasService', () => {
  let service: ReservasService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), ReservasService],
    });

    service = TestBed.inject(ReservasService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should list reservations', () => {
    let response: unknown;

    service.listar().subscribe((value) => {
      response = value;
    });

    const request = httpMock.expectOne('/api/Reservas');
    expect(request.request.method).toBe('GET');

    request.flush([
      {
        reservaId: 1,
        cantidad: '2',
      },
    ]);

    expect(response).toEqual([
      {
        reservaId: 1,
        id: 1,
        cantidad: 2,
      },
    ]);
  });

  it('should get a reservation by id', () => {
    service.obtenerPorId(4).subscribe();

    const request = httpMock.expectOne('/api/Reservas/4');
    expect(request.request.method).toBe('GET');

    request.flush({
      reservaId: 4,
      cantidad: 1,
    });
  });

  it('should list reservations by user id', () => {
    service.listarPorUsuario(3).subscribe();

    const request = httpMock.expectOne('/api/Reservas/usuario/3');
    expect(request.request.method).toBe('GET');

    request.flush([]);
  });

  it('should list reservations for the authenticated user', () => {
    service.listarMisReservas().subscribe();

    const request = httpMock.expectOne('/api/Reservas/mis-reservas');
    expect(request.request.method).toBe('GET');

    request.flush([]);
  });

  it('should create a reservation', () => {
    const payload = {
      eventoId: 5,
      cantidad: 3,
    };

    service.crear(payload).subscribe();

    const request = httpMock.expectOne('/api/Reservas');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);

    request.flush({
      reservaId: 8,
      cantidad: 3,
    });
  });

  it('should confirm payment', () => {
    service.confirmarPago(9).subscribe();

    const request = httpMock.expectOne('/api/Reservas/9/confirmar-pago');
    expect(request.request.method).toBe('PATCH');
    expect(request.request.body).toEqual({});

    request.flush({});
  });

  it('should cancel a reservation', () => {
    service.cancelar(10).subscribe();

    const request = httpMock.expectOne('/api/Reservas/10/cancelar');
    expect(request.request.method).toBe('PATCH');
    expect(request.request.body).toEqual({});

    request.flush({});
  });
});
