import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { PublicMenuComponent } from '../../public-menu/public-menu.component';

@Component({
  selector: 'app-public-layout',
  imports: [PublicMenuComponent, RouterOutlet],
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss',
})
export class PublicLayoutComponent {}
