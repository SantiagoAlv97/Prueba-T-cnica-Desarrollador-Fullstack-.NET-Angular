import { CommonModule, DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';

import { Usuario } from '../../../core/models/usuario.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-account-section',
  imports: [CommonModule, DatePipe],
  templateUrl: './account-section.component.html',
  styleUrl: './account-section.component.scss',
})
export class AccountSectionComponent {
  private readonly authService = inject(AuthService);

  protected readonly usuario = signal<Usuario | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly inicial = computed(() => {
    const nombre = this.usuario()?.nombre?.trim();
    const email = this.usuario()?.email?.trim();
    const referencia = nombre || email || 'U';

    return referencia.slice(0, 1).toUpperCase();
  });

  constructor() {
    this.authService.obtenerPerfil().subscribe({
      next: (usuario) => {
        this.usuario.set(usuario);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No fue posible cargar la informacion de tu cuenta.');
        this.cargando.set(false);
      },
    });
  }
}
