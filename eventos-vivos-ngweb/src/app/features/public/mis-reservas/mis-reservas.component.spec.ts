import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { MisReservasComponent } from './mis-reservas.component';
import { AuthService } from '../../../core/services/auth.service';
import { ReservasService } from '../../../core/services/reservas.service';
import { ToastService } from '../../../core/services/toast.service';

describe('MisReservasComponent', () => {
  let authService: { getUsuario: ReturnType<typeof vi.fn> };
  let reservasService: {
    listarPorUsuario: ReturnType<typeof vi.fn>;
    cancelar: ReturnType<typeof vi.fn>;
  };
  let toastService: { success: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    authService = {
      getUsuario: vi.fn().mockReturnValue({ usuarioId: 8 }),
    };
    reservasService = {
      listarPorUsuario: vi.fn().mockReturnValue(
        of([
          {
            id: 1,
            reservaId: 1,
            cantidad: 2,
            estadoReserva: 'pendiente_pago',
            eventoTitulo: 'Conferencia Angular',
          },
        ]),
      ),
      cancelar: vi.fn().mockReturnValue(of(void 0)),
    };
    toastService = {
      success: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [MisReservasComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
        { provide: ReservasService, useValue: reservasService },
        { provide: ToastService, useValue: toastService },
      ],
    }).compileComponents();
  });

  it('should create the component and load reservations for the logged user', () => {
    const fixture = TestBed.createComponent(MisReservasComponent);

    expect(fixture.componentInstance).toBeTruthy();
    expect(reservasService.listarPorUsuario).toHaveBeenCalledWith(8);
    expect(
      (fixture.componentInstance as never as { totalReservas: () => number }).totalReservas(),
    ).toBe(1);
  });

  it('should cancel a reservation and show a success toast', () => {
    const fixture = TestBed.createComponent(MisReservasComponent);
    const component = fixture.componentInstance as never as {
      cancelar: (reserva: { id: number; reservaId: number }) => void;
      reservas: () => Array<{ estadoReserva?: string }>;
    };

    component.cancelar({ id: 1, reservaId: 1 });

    expect(reservasService.cancelar).toHaveBeenCalledWith(1);
    expect(component.reservas()[0].estadoReserva).toBe('cancelada');
    expect(toastService.success).toHaveBeenCalledWith('Reserva cancelada correctamente.');
  });
});
