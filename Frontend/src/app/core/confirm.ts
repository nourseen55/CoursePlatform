
import { Injectable, signal } from '@angular/core';

export interface ConfirmOptions {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  danger?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  options = signal<ConfirmOptions | null>(null);
  private resolver: ((value: boolean) => void) | null = null;

  ask(options: ConfirmOptions): Promise<boolean> {
    this.resolver?.(false);          // لو فيه نافذة مفتوحة، نقفلها بـ "لأ"
    this.options.set(options);

    return new Promise<boolean>(resolve => {
      this.resolver = resolve;
    });
  }

  answer(value: boolean) {
    this.resolver?.(value);
    this.resolver = null;
    this.options.set(null);
  }
}