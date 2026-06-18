import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';

import { VenuesService } from './venues.service';

describe('VenuesService', () => {
  let service: VenuesService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), VenuesService],
    });

    service = TestBed.inject(VenuesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should list venues', () => {
    service.listar().subscribe();

    const request = httpMock.expectOne('/api/Venues');
    expect(request.request.method).toBe('GET');

    request.flush([]);
  });

  it('should get a venue by id', () => {
    service.obtenerPorId(2).subscribe();

    const request = httpMock.expectOne('/api/Venues/2');
    expect(request.request.method).toBe('GET');

    request.flush({
      venueId: 2,
      nombre: 'Auditorio',
      capacidad: 100,
      ciudad: 'Bogota',
    });
  });

  it('should create a venue', () => {
    const payload = {
      nombre: 'Sala Norte',
      capacidad: 80,
      ciudad: 'Bogota',
    };

    service.crear(payload).subscribe();

    const request = httpMock.expectOne('/api/Venues');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);

    request.flush({
      venueId: 9,
      ...payload,
    });
  });

  it('should update a venue', () => {
    const payload = {
      nombre: 'Sala Norte',
      capacidad: 90,
      ciudad: 'Bogota',
    };

    service.actualizar(9, payload).subscribe();

    const request = httpMock.expectOne('/api/Venues/9');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(payload);

    request.flush({
      venueId: 9,
      ...payload,
    });
  });

  it('should delete a venue', () => {
    service.eliminar(9).subscribe();

    const request = httpMock.expectOne('/api/Venues/9');
    expect(request.request.method).toBe('DELETE');

    request.flush({});
  });
});
