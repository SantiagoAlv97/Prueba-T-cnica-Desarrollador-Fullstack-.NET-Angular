import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';

import { EventosListaComponent } from './eventos-lista.component';
import { EventosService } from '../../../core/services/eventos.service';
import { TiposEventoService } from '../../../core/services/tipos-evento.service';
import { VenuesService } from '../../../core/services/venues.service';

describe('EventosListaComponent', () => {
  let eventosService: { listar: ReturnType<typeof vi.fn> };
  let tiposEventoService: { listar: ReturnType<typeof vi.fn> };
  let venuesService: { listar: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    vi.useFakeTimers();
    eventosService = {
      listar: vi.fn().mockReturnValue(
        of([
          {
            id: 1,
            eventoId: 1,
            titulo: 'Conferencia Angular',
          },
        ]),
      ),
    };
    tiposEventoService = {
      listar: vi.fn().mockReturnValue(of([])),
    };
    venuesService = {
      listar: vi.fn().mockReturnValue(of([])),
    };

    await TestBed.configureTestingModule({
      imports: [EventosListaComponent],
      providers: [
        provideRouter([]),
        { provide: EventosService, useValue: eventosService },
        { provide: TiposEventoService, useValue: tiposEventoService },
        { provide: VenuesService, useValue: venuesService },
      ],
    }).compileComponents();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('should create the component and load events on init', () => {
    const fixture = TestBed.createComponent(EventosListaComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance).toBeTruthy();
    expect(eventosService.listar).toHaveBeenCalled();
    expect((fixture.componentInstance as never as { eventos: () => unknown[] }).eventos().length).toBe(1);
  });

  it('should set an error message when event loading fails', () => {
    eventosService.listar.mockReturnValueOnce(throwError(() => new Error('boom')));
    const fixture = TestBed.createComponent(EventosListaComponent);
    fixture.detectChanges();

    expect((fixture.componentInstance as never as { error: () => string }).error()).toBe(
      'No fue posible cargar los eventos en este momento.',
    );
  });
});
