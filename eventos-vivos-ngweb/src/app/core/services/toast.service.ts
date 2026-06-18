import { Injectable, signal } from '@angular/core';

import { Toast, ToastType } from '../models/toast.model';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly idSequence = signal(0);
  private readonly timeouts = new Map<number, number>();
  private readonly toastsSignal = signal<Toast[]>([]);

  readonly toasts = this.toastsSignal.asReadonly();

  success(message: string): void {
    this.push('success', message);
  }

  error(message: string): void {
    this.push('error', message);
  }

  warning(message: string): void {
    this.push('warning', message);
  }

  info(message: string): void {
    this.push('info', message);
  }

  remove(id: number): void {
    const timeoutId = this.timeouts.get(id);

    if (timeoutId !== undefined) {
      window.clearTimeout(timeoutId);
      this.timeouts.delete(id);
    }

    this.toastsSignal.update((toasts) => toasts.filter((toast) => toast.id !== id));
  }

  clear(): void {
    for (const timeoutId of this.timeouts.values()) {
      window.clearTimeout(timeoutId);
    }

    this.timeouts.clear();
    this.toastsSignal.set([]);
  }

  private push(type: ToastType, message: string, duration = 4000): void {
    const trimmedMessage = message.trim();

    if (!trimmedMessage) {
      return;
    }

    const id = this.idSequence() + 1;
    this.idSequence.set(id);

    const toast: Toast = {
      id,
      type,
      message: trimmedMessage,
      duration,
    };

    this.toastsSignal.update((toasts) => [...toasts, toast]);

    const timeoutId = window.setTimeout(() => {
      this.remove(id);
    }, duration);

    this.timeouts.set(id, timeoutId);
  }
}
