import { NativeDateAdapter } from '@angular/material/core';

/**
 * Custom date adapter and formats for Angular Material date pickers.
 *
 * Supports the "YYYY-MM-DD" format.
 *
 * Used in conjunction with `CustomDateAdapter`.
 *
 * @example
 * ```typescript
 * providers: [
 *   { provide: DateAdapter, useClass: CustomDateAdapter, deps: [MAT_DATE_LOCALE] },
 *   { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS }
 * ]
 * ```
 *
 * @example
 * ```html
 * <input
 *   matInput
 *   formControlName="dateOfBirth"
 *   placeholder="yyyy-mm-dd"
 *   style="padding-left: 12px; border: 1px solid #ccc; border-radius: 0"
 *   class="form-control"
 *   [max]="maxDate"
 *   [min]="minDate"
 *   [matDatepicker]="dateOfBirthPicker"
 *   (focus)="dateOfBirthPicker.open()"
 *   (click)="dateOfBirthPicker.open()"
 * />
 * <mat-datepicker-toggle matSuffix [for]="dateOfBirthPicker"></mat-datepicker-toggle>
 * <mat-datepicker #dateOfBirthPicker></mat-datepicker>
 * ```
 */
export const CUSTOM_DATE_FORMATS = {
  parse: {
    dateInput: 'input'
  },
  display: {
    dateInput: 'input',
    monthYearLabel: 'monthYearLabel',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'monthYearA11yLabel'
  }
};

/**
 * Custom date adapter and formats for Angular Material date pickers.
 *
 * Supports the "YYYY-MM-DD" format.
 *
 * Used in conjunction with `CUSTOM_DATE_FORMATS`.
 *
 * @example
 * ```typescript
 * providers: [
 *   { provide: DateAdapter, useClass: CustomDateAdapter, deps: [MAT_DATE_LOCALE] },
 *   { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS }
 * ]
 * ```
 *
 * @example
 * ```html
 * <input
 *   matInput
 *   formControlName="dateOfBirth"
 *   placeholder="yyyy-mm-dd"
 *   style="padding-left: 12px; border: 1px solid #ccc; border-radius: 0"
 *   class="form-control"
 *   [max]="maxDate"
 *   [min]="minDate"
 *   [matDatepicker]="dateOfBirthPicker"
 *   (focus)="dateOfBirthPicker.open()"
 *   (click)="dateOfBirthPicker.open()"
 * />
 * <mat-datepicker-toggle matSuffix [for]="dateOfBirthPicker"></mat-datepicker-toggle>
 * <mat-datepicker #dateOfBirthPicker></mat-datepicker>
 * ```
 *
 * @export
 * @class CustomDateAdapter
 * @extends {NativeDateAdapter}
 */
export class CustomDateAdapter extends NativeDateAdapter {
  override format(date: Date, displayFormat: any): string {
    if (displayFormat === 'input') {
      const year = date.getFullYear();
      const month = (date.getMonth() + 1).toString().padStart(2, '0');
      const day = date.getDate().toString().padStart(2, '0');
      return `${year}-${month}-${day}`;
    }

    // Handle month/year label in datepicker header
    if (displayFormat === 'monthYearLabel') {
      const year = date.getFullYear();
      const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      const month = monthNames[date.getMonth()];
      return `${year}-${month}`;
    }

    // Handle month/year accessibility label
    if (displayFormat === 'monthYearA11yLabel') {
      const year = date.getFullYear();
      const monthNames = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December'
      ];
      const month = monthNames[date.getMonth()];
      return `${year} ${month}`;
    }

    return super.format(date, displayFormat);
  }

  override parse(value: any): Date | null {
    if (typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}$/)) {
      const [year, month, day] = value.split('-').map(Number);
      return new Date(year, month - 1, day);
    }
    return super.parse(value);
  }
}
