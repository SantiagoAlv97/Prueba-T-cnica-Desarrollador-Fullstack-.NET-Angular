import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';

import { ToastService } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    vi.useFakeTimers();
    TestBed.configureTestingModule({
      providers: [ToastService],
    });

    service = TestBed.inject(ToastService);
  });

  afterEach(() => {
    service.clear();
    vi.useRealTimers();
  });

  it('should add success, error, warning and info toasts', () => {
    service.success('Operacion correcta');
    service.error('Ocurrio un error');
    service.warning('Mensaje de alerta');
    service.info('Mensaje informativo');

    expect(service.toasts().map((toast) => toast.type)).toEqual([
      'success',
      'error',
      'warning',
      'info',
    ]);
  });

  it('should remove a toast manually', () => {
    service.success('Operacion correcta');
    const [toast] = service.toasts();

    service.remove(toast.id);

    expect(service.toasts()).toEqual([]);
  });

  it('should clear all toasts', () => {
    service.success('Operacion correcta');
    service.error('Ocurrio un error');

    service.clear();

    expect(service.toasts()).toEqual([]);
  });

  it('should auto-remove toasts after the default duration', () => {
    service.success('Operacion correcta');

    expect(service.toasts().length).toBe(1);

    vi.advanceTimersByTime(4000);

    expect(service.toasts().length).toBe(0);
  });
});
