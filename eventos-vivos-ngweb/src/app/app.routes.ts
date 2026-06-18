import { Routes } from '@angular/router';
import { adminGuard } from './core/guards/admin.guard';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'eventos',
    pathMatch: 'full',
  },
  {
    path: '',
    loadComponent: () =>
      import('./shared/components/layouts/public-layout/public-layout.component').then(
        (c) => c.PublicLayoutComponent,
      ),
    children: [
      {
        path: 'eventos',
        loadComponent: () =>
          import('./features/public/eventos-lista/eventos-lista.component').then(
            (c) => c.EventosListaComponent,
          ),
      },
      {
        path: 'eventos/:id',
        loadComponent: () =>
          import('./features/public/evento-detalle/evento-detalle.component').then(
            (c) => c.EventoDetalleComponent,
          ),
      },
      {
        path: 'eventos/:id/reservar',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/public/reserva-form/reserva-form.component').then(
            (c) => c.ReservaFormComponent,
          ),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then(
            (c) => c.LoginComponent,
          ),
      },
      {
        path: 'mis-reservas',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/public/mis-reservas/mis-reservas.component').then(
            (c) => c.MisReservasComponent,
          ),
      },
      {
        path: 'mi-cuenta',
        canActivate: [authGuard],
        data: {
          title: 'Mis datos',
          description:
            'Aqui se mostraran los datos de perfil y preferencias de la cuenta autenticada.',
        },
        loadComponent: () =>
          import('./features/account/account-section-placeholder/account-section-placeholder.component').then(
            (c) => c.AccountSectionPlaceholderComponent,
          ),
      },
    ],
  },
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./shared/components/layouts/admin-layout/admin-layout.component').then(
        (c) => c.AdminLayoutComponent,
      ),
    children: [
      {
        path: 'eventos',
        loadComponent: () =>
          import('./features/admin/eventos-admin/eventos-admin.component').then(
            (c) => c.EventosAdminComponent,
          ),
      },
      {
        path: 'venues',
        loadComponent: () =>
          import('./features/admin/venues-admin/venues-admin.component').then(
            (c) => c.VenuesAdminComponent,
          ),
      },
      {
        path: 'reservas',
        loadComponent: () =>
          import('./features/admin/reservas-admin/reservas-admin.component').then(
            (c) => c.ReservasAdminComponent,
          ),
      },
      {
        path: 'reportes',
        loadComponent: () =>
          import('./features/admin/reportes-admin/reportes-admin.component').then(
            (c) => c.ReportesAdminComponent,
          ),
      },
    ],
  },
];
