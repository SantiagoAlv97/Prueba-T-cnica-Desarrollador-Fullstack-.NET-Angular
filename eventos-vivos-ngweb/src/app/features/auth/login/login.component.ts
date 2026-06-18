import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements AfterViewInit {
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);
  private readonly googleButton = viewChild<ElementRef<HTMLDivElement>>('googleButton');

  protected readonly cargando = signal(false);

  async ngAfterViewInit(): Promise<void> {
    if (this.authService.isAuthenticated()) {
      await this.redirigirSegunRol();
      return;
    }

    try {
      await this.esperarGoogleIdentity();
      this.inicializarGoogle();
    } catch {
      this.toastService.error('No fue posible cargar el acceso con Google. Intenta nuevamente.');
    }
  }

  private inicializarGoogle(): void {
    const googleButtonElement = this.googleButton()?.nativeElement;

    if (!googleButtonElement || typeof google === 'undefined') {
      this.toastService.error('Google Sign-In no está disponible en este momento.');
      return;
    }

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: { credential?: string }) => {
        const idToken = response.credential?.trim();

        if (!idToken) {
          this.toastService.error('No se recibió la credencial de Google.');
          return;
        }

        this.iniciarSesion(idToken);
      },
    });

    google.accounts.id.renderButton(googleButtonElement, {
      theme: 'outline',
      size: 'large',
      width: '320',
      text: 'continue_with',
      shape: 'rectangular',
    });
  }

  private iniciarSesion(idToken: string): void {
    this.cargando.set(true);

    this.authService.loginWithGoogle(idToken).subscribe({
      next: async () => {
        this.cargando.set(false);
        this.toastService.success('Inicio de sesión correcto.');
        await this.redirigirSegunRol();
      },
      error: () => {
        this.cargando.set(false);
      },
    });
  }

  private async redirigirSegunRol(): Promise<void> {
    const destino = this.authService.isAdmin() ? '/admin/eventos' : '/eventos';
    await this.router.navigateByUrl(destino);
  }

  private esperarGoogleIdentity(): Promise<void> {
    if (typeof google !== 'undefined') {
      return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
      const script = document.querySelector<HTMLScriptElement>(
        'script[src="https://accounts.google.com/gsi/client"]',
      );

      if (!script) {
        reject(new Error('missing-google-script'));
        return;
      }

      const onLoad = (): void => {
        script.removeEventListener('load', onLoad);
        script.removeEventListener('error', onError);
        resolve();
      };

      const onError = (): void => {
        script.removeEventListener('load', onLoad);
        script.removeEventListener('error', onError);
        reject(new Error('google-script-error'));
      };

      script.addEventListener('load', onLoad, { once: true });
      script.addEventListener('error', onError, { once: true });
    });
  }
}
