import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AdminMenuComponent } from '../../admin-menu/admin-menu.component';

@Component({
  selector: 'app-admin-layout',
  imports: [AdminMenuComponent, RouterOutlet],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {}
