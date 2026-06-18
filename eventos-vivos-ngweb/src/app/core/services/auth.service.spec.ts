import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { vi } from 'vitest';

import { AuthService } from './auth.service';

function createToken(expirationSecondsFromNow = 3600): string {
  const header = { alg: 'HS256', typ: 'JWT' };
  const payload = {
    exp: Math.floor(Date.now() / 1000) + expirationSecondsFromNow,
  };

  const encode = (value: object): string =>
    btoa(JSON.stringify(value)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '');

  return `${encode(header)}.${encode(payload)}.signature`;
}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let router: { navigateByUrl: ReturnType<typeof vi.fn>; url: string };

  beforeEach(() => {
    localStorage.clear();
    router = {
      navigateByUrl: vi.fn().mockResolvedValue(true),
      url: '/eventos',
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AuthService,
        { provide: Router, useValue: router },
      ],
    });
  });

  afterEach(() => {
    httpMock?.verify();
    localStorage.clear();
  });

  it('should login with Google and persist the normalized user', () => {
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    const token = createToken();

    service.loginWithGoogle('google-id-token').subscribe((response) => {
      expect(response.token).toBe(token);
    });

    const request = httpMock.expectOne('/api/auth/google');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ idToken: 'google-id-token' });

    request.flush({
      token,
      usuarioId: 7,
      nombre: 'Ada',
      email: 'ada@example.com',
      rol: 'administrador',
      FotoURL: 'https://cdn.example.com/ada.png',
    });

    expect(localStorage.getItem('access_token')).toBe(token);
    expect(service.isAuthenticated()).toBe(true);
    expect(service.isAdmin()).toBe(true);
    expect(service.getUsuario()).toEqual(
      expect.objectContaining({
        usuarioId: 7,
        nombre: 'Ada',
        email: 'ada@example.com',
        rol: 'administrador',
        fotoUrl: 'https://cdn.example.com/ada.png',
      }),
    );
  });

  it('should logout and redirect to login', () => {
    localStorage.setItem('access_token', createToken());
    localStorage.setItem(
      'usuario',
      JSON.stringify({
        usuarioId: 7,
        nombre: 'Ada',
      }),
    );

    service = TestBed.inject(AuthService);
    service.logout();

    expect(localStorage.getItem('access_token')).toBeNull();
    expect(localStorage.getItem('usuario')).toBeNull();
    expect(router.navigateByUrl).toHaveBeenCalledWith('/login');
  });

  it('should load the authenticated profile and refresh the cached user', () => {
    localStorage.setItem('access_token', createToken());
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);

    service.obtenerPerfil().subscribe((usuario) => {
      expect(usuario.email).toBe('ada@example.com');
    });

    const request = httpMock.expectOne('/api/auth/me');
    expect(request.request.method).toBe('GET');

    request.flush({
      usuarioId: 7,
      nombre: 'Ada',
      email: 'ada@example.com',
      rol: 'administrador',
      fotoUrl: 'https://cdn.example.com/ada.png',
      activo: true,
      fechaCreacion: '2026-06-18T00:00:00Z',
      fechaUltimoAcceso: '2026-06-18T12:00:00Z',
    });

    expect(service.getUsuario()).toEqual(
      expect.objectContaining({
        usuarioId: 7,
        nombre: 'Ada',
        email: 'ada@example.com',
        rol: 'administrador',
      }),
    );
  });

  it('should return an empty token and clear the session when access token is expired', () => {
    localStorage.setItem('access_token', createToken(-60));
    localStorage.setItem(
      'usuario',
      JSON.stringify({
        usuarioId: 7,
        nombre: 'Ada',
      }),
    );

    service = TestBed.inject(AuthService);

    expect(service.getAccessToken()).toBe('');
    expect(service.isAuthenticated()).toBe(false);
    expect(localStorage.getItem('access_token')).toBeNull();
    expect(router.navigateByUrl).not.toHaveBeenCalled();
  });
});
