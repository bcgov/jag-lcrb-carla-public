import { ValidatorFn, AbstractControl, FormControl, FormGroup } from '@angular/forms';


export const CanadaPostalRegex = '^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$';
export const USPostalRegex = '^\\d{5}([\-]\\d{4})?$';

export class FormBase {
    form: FormGroup;

    isValidOrNotTouched(field: string) {
        return this.form.get(field).valid || !this.form.get(field).touched;
    }

    public rejectIfNotDigitOrBackSpace(event) {
        const acceptedKeys = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Control',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'];
        if (acceptedKeys.indexOf(event.key) === -1) {
            event.preventDefault();
        }
    }

    public customRequiredCheckboxValidator(): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            if (control.value === true) {
                return null;
            } else {
                return { 'shouldBeTrue': 'But value is false' };
            }
        };
    }

    public customZipCodeValidator(countryField: string): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            if (!control.parent) {
                return null;
            }
            const country = control.parent.get(countryField).value;
            if (country !== 'Canada' && country !== 'United States of America') {
                return null;
            }
            let pattern = new RegExp(CanadaPostalRegex);
            if (country === 'United States of America') {
                pattern = new RegExp(USPostalRegex);
            }
            const valueMatchesPattern = pattern.test(control.value);
            return valueMatchesPattern ? null : { 'regex-missmatch': { value: control.value } };
        };
    }

    public requiredCheckboxGroupValidator(checkboxFields: string[]): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            if (!control.parent) {
                return null;
            }
            let valid = false;
            checkboxFields.forEach(f => {
                valid = valid || control.parent.get(f).value;
            });
            return valid ? null : { 'required-set': { value: control.value } };
        };
    }

    public requiredCheckboxChildValidator(checkboxField: string): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            if (!control.parent || !control.parent.get(checkboxField)) {
                return null;
            }
            const parentIsChecked = control.parent.get(checkboxField).value;
            if (!parentIsChecked) {
                return null;
            }
            return control.value ? null : { 'required': { value: control.value } };
        };
    }


    public requiredSelectChildValidator(selectField: string, conditionalValue: any[]): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            if (!control.parent
                || !control.parent.get(selectField)
                || conditionalValue.indexOf(control.parent.get(selectField).value) === -1) {
                return null;
            }
            const parentIsChecked = control.parent.get(selectField).value;
            if (!parentIsChecked) {
                return null;
            }
            return (control.value !== null && control.value !== '') ? null : { 'required': { value: control.value } };
        };
    }

    public trimValue(control: AbstractControl) {
        const value = control.value;
        control.setValue('');
        control.setValue(value.trim());
    }
}
