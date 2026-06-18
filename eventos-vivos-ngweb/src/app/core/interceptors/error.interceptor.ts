import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: unknown) => {
      const message = resolveErrorMessage(error);

      if (error instanceof HttpErrorResponse && error.status === 401 && authService.isAuthenticated()) {
        authService.handleSessionExpired();
      }

      toastService.error(message);

      return throwError(() => error);
    }),
  );
};

function resolveErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Ocurrió un error inesperado.';
  }

  const backendMessage = extractBackendMessage(error);

  if (backendMessage) {
    return backendMessage;
  }

  if (error.status === 401) {
    return 'Tu sesión expiró o no estás autenticado.';
  }

  if (error.status === 403) {
    return 'No tienes permisos para realizar esta acción.';
  }

  return 'Ocurrió un error inesperado.';
}

function extractBackendMessage(error: HttpErrorResponse): string {
  const payload = error.error;

  if (payload && typeof payload === 'object') {
    const objectPayload = payload as { message?: unknown; title?: unknown };

    if (typeof objectPayload.message === 'string' && objectPayload.message.trim()) {
      return objectPayload.message.trim();
    }

    if (typeof objectPayload.title === 'string' && objectPayload.title.trim()) {
      return objectPayload.title.trim();
    }
  }

  if (typeof payload === 'string' && payload.trim()) {
    return payload.trim();
  }

  if (typeof error.message === 'string' && error.message.trim()) {
    return error.message.trim();
  }

  return '';
}
