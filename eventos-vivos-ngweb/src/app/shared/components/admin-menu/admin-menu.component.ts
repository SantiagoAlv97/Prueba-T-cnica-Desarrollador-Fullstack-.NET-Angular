import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-menu',
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-menu.component.html',
  styleUrl: './admin-menu.component.scss',
})
export class AdminMenuComponent {
  private readonly authService = inject(AuthService);

  protected readonly usuario = this.authService.usuario;
  protected readonly estaAutenticado = this.authService.estaAutenticado;
  protected readonly nombreUsuario = computed(() => {
    const usuario = this.usuario();
    return usuario?.nombre?.trim() || usuario?.name?.trim() || usuario?.email?.trim() || 'Usuario';
  });
  protected readonly fotoPerfil = computed(() => {
    const usuario = this.usuario();
    return usuario?.fotoUrl?.trim() || usuario?.picture?.trim() || '';
  });

  protected cerrarSesion(): void {
    this.authService.logout();
  }
}
