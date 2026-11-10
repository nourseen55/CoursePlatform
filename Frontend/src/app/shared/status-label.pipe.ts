import { Pipe, PipeTransform } from '@angular/core';

const LABELS: Record<string, string> = {
  Draft: 'مسودة',
  Published: 'منشور',
  Archived: 'مؤرشف',
};

@Pipe({ name: 'statusLabel' })
export class StatusLabelPipe implements PipeTransform {
  transform(status: string): string {
    return LABELS[status] ?? status;
  }
}