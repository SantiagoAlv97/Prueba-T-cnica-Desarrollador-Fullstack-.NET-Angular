import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-admin-section-placeholder',
  templateUrl: './admin-section-placeholder.component.html',
  styleUrl: './admin-section-placeholder.component.scss',
})
export class AdminSectionPlaceholderComponent {
  private readonly route = inject(ActivatedRoute);

  protected readonly titulo = computed(
    () => (this.route.snapshot.data['title'] as string | undefined) ?? 'Seccion admin',
  );

  protected readonly descripcion = computed(
    () =>
      (this.route.snapshot.data['description'] as string | undefined) ??
      'Esta vista quedo reservada para la funcionalidad administrativa.',
  );
}
