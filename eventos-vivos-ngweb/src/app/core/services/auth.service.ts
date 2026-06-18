import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse } from '../models/auth-response.model';
import { UsuarioAuth } from '../models/usuario-auth.model';

type UsuarioAuthInput = Partial<UsuarioAuth> &
  Partial<AuthResponse> & {
    usuarioId?: number | null;
  };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly endpoint = `${environment.apiUrl}/auth/google`;
  private readonly usuarioSignal = signal<UsuarioAuth | null>(this.leerUsuario());
  private readonly accessTokenSignal = signal(this.leerAccessToken());
  private sessionExpirationTimer: ReturnType<typeof setTimeout> | null = null;

  readonly usuario = this.usuarioSignal.asReadonly();
  readonly estaAutenticado = computed(() => Boolean(this.accessTokenSignal()));

  constructor() {
    this.sincronizarSesionActual();
  }

  loginWithGoogle(idToken: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.endpoint, { idToken }).pipe(
      tap((response) => {
        this.accessTokenSignal.set(response.token);
        localStorage.setItem('access_token', response.token);
        this.setUsuario(this.normalizarUsuario(response));
        this.programarExpiracionSesion(response.token);
      }),
    );
  }

  logout(): void {
    this.finalizarSesion(true);
  }

  isAuthenticated(): boolean {
    if (this.tokenExpirado(this.accessTokenSignal())) {
      this.finalizarSesion(false);
      return false;
    }

    return Boolean(this.accessTokenSignal());
  }

  getUsuario(): UsuarioAuth | null {
    return this.usuarioSignal();
  }

  isAdmin(): boolean {
    const rol = this.usuarioSignal()?.rol?.trim().toLowerCase();
    return rol === 'administrador' || rol === 'admin';
  }

  getAccessToken(): string {
    if (this.tokenExpirado(this.accessTokenSignal())) {
      this.handleSessionExpired();
      return '';
    }

    return this.accessTokenSignal();
  }

  handleSessionExpired(): void {
    if (!this.accessTokenSignal()) {
      return;
    }

    this.finalizarSesion(true);
  }

  private setUsuario(usuario: UsuarioAuth | null): void {
    this.usuarioSignal.set(usuario);

    if (usuario) {
      localStorage.setItem('usuario', JSON.stringify(usuario));
      return;
    }

    localStorage.removeItem('usuario');
  }

  private leerAccessToken(): string {
    return localStorage.getItem('access_token') ?? '';
  }

  private sincronizarSesionActual(): void {
    const token = this.accessTokenSignal();

    if (!token) {
      return;
    }

    if (this.tokenExpirado(token)) {
      this.finalizarSesion(false);
      return;
    }

    this.programarExpiracionSesion(token);
  }

  private leerUsuario(): UsuarioAuth | null {
    const rawUsuario = localStorage.getItem('usuario');

    if (!rawUsuario) {
      return null;
    }

    try {
      const usuario = JSON.parse(rawUsuario) as unknown;

      if (!this.esUsuarioValido(usuario)) {
        return null;
      }

      return this.normalizarUsuario(usuario);
    } catch {
      return null;
    }
  }

  private normalizarUsuario(usuario: UsuarioAuthInput): UsuarioAuth {
    const fotoNormalizada =
      usuario.fotoUrl?.trim() ||
      usuario.fotoURL?.trim() ||
      usuario.FotoURL?.trim() ||
      usuario.picture?.trim() ||
      undefined;

    return {
      id: usuario.id ?? usuario.usuarioId ?? undefined,
      usuarioId: usuario.usuarioId ?? undefined,
      nombre: usuario.nombre ?? usuario.name ?? undefined,
      name: usuario.name ?? usuario.nombre ?? undefined,
      email: usuario.email ?? undefined,
      rol: usuario.rol ?? undefined,
      fotoUrl: fotoNormalizada,
      fotoURL: fotoNormalizada,
      FotoURL: fotoNormalizada,
      picture: fotoNormalizada,
    };
  }

  private esUsuarioValido(valor: unknown): valor is UsuarioAuthInput {
    return typeof valor === 'object' && valor !== null;
  }

  private finalizarSesion(redirectToLogin: boolean): void {
    this.limpiarTemporizadorSesion();
    localStorage.removeItem('access_token');
    localStorage.removeItem('usuario');
    this.accessTokenSignal.set('');
    this.usuarioSignal.set(null);

    if (!redirectToLogin || this.router.url === '/login') {
      return;
    }

    void this.router.navigateByUrl('/login');
  }

  private programarExpiracionSesion(token: string): void {
    this.limpiarTemporizadorSesion();

    const fechaExpiracion = this.obtenerFechaExpiracion(token);

    if (!fechaExpiracion) {
      return;
    }

    const tiempoRestante = fechaExpiracion.getTime() - Date.now();

    if (tiempoRestante <= 0) {
      this.handleSessionExpired();
      return;
    }

    this.sessionExpirationTimer = setTimeout(() => {
      this.handleSessionExpired();
    }, tiempoRestante);
  }

  private limpiarTemporizadorSesion(): void {
    if (this.sessionExpirationTimer === null) {
      return;
    }

    clearTimeout(this.sessionExpirationTimer);
    this.sessionExpirationTimer = null;
  }

  private tokenExpirado(token: string | null | undefined): boolean {
    if (!token) {
      return false;
    }

    const fechaExpiracion = this.obtenerFechaExpiracion(token);

    if (!fechaExpiracion) {
      return false;
    }

    return fechaExpiracion.getTime() <= Date.now();
  }

  private obtenerFechaExpiracion(token: string): Date | null {
    const payload = this.decodificarPayloadToken(token);
    const exp = payload?.exp;

    if (typeof exp !== 'number') {
      return null;
    }

    return new Date(exp * 1000);
  }

  private decodificarPayloadToken(token: string): { exp?: number } | null {
    const [, payload] = token.split('.');

    if (!payload) {
      return null;
    }

    try {
      const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
      const paddedPayload = normalizedPayload.padEnd(
        normalizedPayload.length + ((4 - (normalizedPayload.length % 4)) % 4),
        '=',
      );
      const decodedPayload = atob(paddedPayload);
      const parsedPayload = JSON.parse(decodedPayload) as unknown;

      if (typeof parsedPayload !== 'object' || parsedPayload === null) {
        return null;
      }

      return parsedPayload as { exp?: number };
    } catch {
      return null;
    }
  }
}
