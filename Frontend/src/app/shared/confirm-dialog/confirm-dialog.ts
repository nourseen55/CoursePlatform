import { Component, ElementRef, effect, inject, viewChild } from '@angular/core';
import { ConfirmService } from '../../core/confirm';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css',
})
export class ConfirmDialog {
  confirmService = inject(ConfirmService);
  private dialog = viewChild<ElementRef<HTMLDialogElement>>('dlg');

  constructor() {
    effect(() => {
      const el = this.dialog()?.nativeElement;
      if (!el) return;

      if (this.confirmService.options()) {
        if (!el.open) el.showModal();
      } else if (el.open) {
        el.close();
      }
    });
  }

  onBackdropClick(event: MouseEvent) {
    if (event.target === this.dialog()?.nativeElement) {
      this.confirmService.answer(false);
    }
  }
}