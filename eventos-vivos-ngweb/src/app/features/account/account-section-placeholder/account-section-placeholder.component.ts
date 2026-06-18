import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-account-section-placeholder',
  templateUrl: './account-section-placeholder.component.html',
  styleUrl: './account-section-placeholder.component.scss',
})
export class AccountSectionPlaceholderComponent {
  private readonly route = inject(ActivatedRoute);

  protected readonly titulo = computed(
    () => (this.route.snapshot.data['title'] as string | undefined) ?? 'Mi cuenta',
  );

  protected readonly descripcion = computed(
    () =>
      (this.route.snapshot.data['description'] as string | undefined) ??
      'Esta vista quedo lista para conectar informacion del usuario autenticado.',
  );
}
