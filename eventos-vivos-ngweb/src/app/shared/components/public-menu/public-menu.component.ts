import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-public-menu',
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './public-menu.component.html',
  styleUrl: './public-menu.component.scss',
})
export class PublicMenuComponent {
  private readonly authService = inject(AuthService);

  protected readonly usuario = this.authService.usuario;
  protected readonly estaAutenticado = this.authService.estaAutenticado;
  protected readonly perfilAbierto = signal(false);
  protected readonly nombreUsuario = computed(() => {
    const usuario = this.usuario();
    return usuario?.nombre?.trim() || usuario?.name?.trim() || usuario?.email?.trim() || 'Usuario';
  });
  protected readonly fotoPerfil = computed(() => {
    const usuario = this.usuario();
    return usuario?.fotoUrl?.trim() || usuario?.picture?.trim() || '';
  });

  protected alternarPerfil(): void {
    this.perfilAbierto.update((abierto) => !abierto);
  }

  protected cerrarPerfil(): void {
    this.perfilAbierto.set(false);
  }

  protected cerrarSesion(): void {
    this.perfilAbierto.set(false);
    this.authService.logout();
  }
}
