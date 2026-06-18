import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';

import { Toast, ToastType } from '../../../core/models/toast.model';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  imports: [CommonModule],
  templateUrl: './toast-container.component.html',
  styleUrl: './toast-container.component.scss',
})
export class ToastContainerComponent {
  private readonly toastService = inject(ToastService);

  protected readonly toasts = this.toastService.toasts;

  protected trackByToast(_: number, toast: Toast): number {
    return toast.id;
  }

  protected remove(id: number): void {
    this.toastService.remove(id);
  }

  protected getClasses(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'border-[#0052D1] bg-[#EAF2FF] text-black';
      case 'error':
        return 'border-[#93000A] bg-[#FFF1F1] text-[#93000A]';
      case 'warning':
        return 'border-[#7A5A00] bg-[#FFF7DB] text-[#5C4300]';
      case 'info':
        return 'border-[#0052D1] bg-[#EDF4FF] text-[#0052D1]';
      default:
        return 'border-black/30 bg-white text-black';
    }
  }

  protected getLabel(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'Correcto';
      case 'error':
        return 'Error';
      case 'warning':
        return 'Atencion';
      case 'info':
        return 'Info';
      default:
        return 'Mensaje';
    }
  }

  protected getCloseButtonClasses(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'border-[#0052D1] bg-white text-[#0052D1] hover:bg-[#DCEAFF]';
      case 'error':
        return 'border-[#93000A] bg-white text-[#93000A] hover:bg-[#FFE2E2]';
      case 'warning':
        return 'border-[#7A5A00] bg-white text-[#5C4300] hover:bg-[#F6E7B8]';
      case 'info':
        return 'border-[#0052D1] bg-white text-[#0052D1] hover:bg-[#DCEAFF]';
      default:
        return 'border-black/30 bg-white text-black hover:bg-[#F2F2F2]';
    }
  }
}
