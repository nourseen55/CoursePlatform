import { HttpErrorResponse } from '@angular/common/http';

export function errorText(err: HttpErrorResponse): string {
  if (err.status === 0) return 'مش قادر أوصل للسيرفر.';
  if (err.status === 401) return 'لازم تسجلي دخول من جديد.';
  if (err.status === 403) return 'ملكيش صلاحية للعملية دي.';
  if (err.status === 404) return 'العنصر مش موجود.';

  const validation = err.error?.errors;
  if (validation) {
    return Object.values<string[]>(validation).flat().join(' ');
  }

  return typeof err.error === 'string' ? err.error : 'حصل خطأ غير متوقع.';
}