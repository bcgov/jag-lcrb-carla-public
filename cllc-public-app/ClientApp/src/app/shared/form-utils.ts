import { FormGroup } from '@angular/forms';

/**
 * Returns `true` if a form control is disabled, valid, or not touched.
 *
 * @export
 * @param {FormGroup} form
 * @param {string} field
 * @return {*}  {boolean}
 */
export function isValidOrNotTouched(form: FormGroup, field: string): boolean {
  const control = form.get(field);

  return !!(control?.disabled || control?.valid || !control?.touched);
}
