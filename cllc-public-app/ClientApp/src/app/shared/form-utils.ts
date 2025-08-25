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
  if (!form || !field) {
    return false;
  }

  const control = form.get(field);

  if (!control) {
    return false;
  }

  return !!(control?.disabled || control?.valid || !control?.touched);
}

/**
 * Returns `true` if the form is disabled, valid, or not touched.
 *
 * @export
 * @param {FormGroup} form
 * @return {*}  {boolean}
 */
export function isFormValidOrNotTouched(form: FormGroup): boolean {
  return isFormValid(form) || !form.touched;
}

/**
 * Returns `true` if the form is disabled or valid.
 *
 * @export
 * @param {FormGroup} form
 * @return {*}  {boolean}
 */
export function isFormValid(form: FormGroup): boolean {
  if (!form) {
    return false;
  }

  return !!(form.disabled || form.valid);
}
