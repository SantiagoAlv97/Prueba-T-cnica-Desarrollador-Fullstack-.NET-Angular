import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { ReservasAdminComponent } from './reservas-admin.component';
import { ReservasService } from '../../../core/services/reservas.service';
import { ToastService } from '../../../core/services/toast.service';

describe('ReservasAdminComponent', () => {
  let reservasService: {
    listar: ReturnType<typeof vi.fn>;
    confirmarPago: ReturnType<typeof vi.fn>;
    cancelar: ReturnType<typeof vi.fn>;
  };
  let toastService: { success: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    reservasService = {
      listar: vi.fn().mockReturnValue(
        of([
          {
            id: 1,
            reservaId: 1,
            cantidad: 2,
            estadoReservaId: 1,
            estadoReserva: 'pendiente_pago',
            eventoTitulo: 'Conferencia Angular',
          },
        ]),
      ),
      confirmarPago: vi.fn().mockReturnValue(of(void 0)),
      cancelar: vi.fn().mockReturnValue(of(void 0)),
    };
    toastService = {
      success: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [ReservasAdminComponent],
      providers: [
        { provide: ReservasService, useValue: reservasService },
        { provide: ToastService, useValue: toastService },
      ],
    }).compileComponents();
  });

  it('should create the component and load reservations', () => {
    const fixture = TestBed.createComponent(ReservasAdminComponent);

    expect(fixture.componentInstance).toBeTruthy();
    expect(reservasService.listar).toHaveBeenCalled();
  });

  it('should confirm payment and show a success toast', () => {
    const fixture = TestBed.createComponent(ReservasAdminComponent);
    const component = fixture.componentInstance as never as {
      confirmarPago: (reserva: { id: number; reservaId: number }) => void;
    };

    component.confirmarPago({ id: 1, reservaId: 1 });

    expect(reservasService.confirmarPago).toHaveBeenCalledWith(1);
    expect(toastService.success).toHaveBeenCalledWith('Pago confirmado correctamente.');
  });
});
