import { ValidatorFn, AbstractControl, FormGroup, FormArray } from '@angular/forms';
import { Injectable, OnDestroy } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';
import { Account } from '@models/account.model';
import { Subscription } from 'rxjs';


export const CanadaPostalRegex = '^[A-Za-z][0-9][A-Za-z] ?[0-9][A-Za-z][0-9]$';
export const USPostalRegex = '^\\d{5}([\-]\\d{4})?$';

export class ApplicationHTMLContent {
  title: string;
  preamble: string;
  beforeStarting: string;
  nextSteps: string;
  LocalGovernmentApproval: string;
  floorPlan: string;
  sitePlan: string;
  sitePhotos: string;
  validInterest: string;
  letterOfIntent: string;
  zoning: string;
  serviceAreas: string;
  outdoorAreas: string;
  capacityArea: string;
  signage: string;
  grocery: string;
  patio: string;
  CannabisAssociateContent: string;
  LiquorPHSContent: string;
  paymentConfirmation: string;

}

@Injectable()
export class FormBase implements OnDestroy {
  form: FormGroup;
  account: Account;
  componentActive = true;
  application: Application;
  htmlContent: ApplicationHTMLContent;
  ApplicationTypeNames = ApplicationTypeNames;
  subscriptionList: Subscription[] = [];

  addDynamicContent() {
    if (this.application.applicationType) {
      this.htmlContent = {
        title: this.application.applicationType.title,
        preamble: this.getApplicationContent('Preamble'),
        beforeStarting: this.getApplicationContent('BeforeStarting'),
        nextSteps: this.getApplicationContent('NextSteps'),
        LocalGovernmentApproval: this.getApplicationContent('LocalGovernmentApproval'),
        floorPlan: this.getApplicationContent('FloorPlan'),
        sitePlan: this.getApplicationContent('SitePlan'),
        sitePhotos: this.getApplicationContent('SitePhotos'),
        validInterest: this.getApplicationContent('ValidInterest'),
        letterOfIntent: this.getApplicationContent('LetterOfIntent'),
        zoning: this.getApplicationContent('Zoning'),
        serviceAreas: this.getApplicationContent('ServiceArea'),
        outdoorAreas: this.getApplicationContent('OutdoorArea'),
        capacityArea: this.getApplicationContent('CapacityArea'),
        signage: this.getApplicationContent('Signage'),
        grocery: this.getApplicationContent('Grocery'),
        CannabisAssociateContent: this.getApplicationContent('CannabisAssociateContent'),
        LiquorPHSContent: this.getApplicationContent('LiquorPHSContent'),
        paymentConfirmation: this.getApplicationContent('LicencepaymentConfirmation'),
        patio: this.getApplicationContent('Patio')
      };
    }
  }

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.ReadOnly.toString()]
      .indexOf(state) !== -1;
  }

  getApplicationContent(contentCartegory: string) {
    let body = '';
    if (this.application.applicationType.contentTypes) {
      const contents =
        this.application.applicationType.contentTypes
          .filter(t => t.category === contentCartegory
            && (t.businessTypes.indexOf(this.application.applicantType) !== -1
              || t.businessTypes.indexOf(this.account && this.account.businessType) !== -1)
          );
      if (contents.length > 0) {
        body = contents[0].body;
      }
    }
    return body;
  }

  isValidOrNotTouched(field: string) {
    return this.form.get(field).disabled
      || this.form.get(field).valid
      || !this.form.get(field).touched;
  }

  rejectIfNotDigitOrBackSpace(event) {
    const acceptedKeys = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Control',
      '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'];
    if (acceptedKeys.indexOf(event.key) === -1) {
      event.preventDefault();
    }
  }

  customRequiredCheckboxValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (control.value === true) {
        return null;
      } else {
        return { 'shouldBeTrue': 'But value is false' };
      }
    };
  }

  customZipCodeValidator(countryField: string): ValidatorFn {
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

  requiredCheckboxGroupValidator(checkboxFields: string[]): ValidatorFn {
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

  requireOneOfGroupValidator(fields: string[]): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.parent) {
        return null;
      }
      let valid = false;
      fields.forEach(f => {
        valid = valid || !!control.parent.get(f).value;
      });
      return valid ? null : { 'require-one-of': { value: control.value } };
    };
  }

  requiredCheckboxChildValidator(checkboxField: string): ValidatorFn {
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

  requiredAutoCompleteId(control: AbstractControl): { [key: string]: any } | null {
    let result = { 'required': { value: control.value } }; // invalid by default
    if (control.value && control.value.id) {
      result = null; // valid
    }
    return result;
  }


  requiredSelectChildValidator(selectField: string, conditionalValue: any[]): ValidatorFn {
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

  trimValue(control: AbstractControl) {
    const value = control.value;
    control.setValue('');
    control.setValue(value.trim());
  }

  isTouchedAndInvalid(fieldName: string): boolean {
    return this.form.get(fieldName).touched
      && !this.form.get(fieldName).valid;
  }

  ngOnDestroy(): void {
    this.componentActive = false;
    this.subscriptionList.forEach(sub => {
      sub.unsubscribe();
    });
  }

  listControlsWithErrors(form: FormGroup | FormArray, ValidationFieldNameMap: any = {}, parentName: string = ''): string[] {
    let list = [];
    // list errors at the form level
    if (form instanceof FormGroup) {
      // tslint:disable-next-line: forin
      for (const error in form.errors) {
        list.push(`${parentName} ${form.errors[error]}`);
      }

      for (const c in form.controls) {
        if (c) {
          const control = form.get(c);
          if (!control.valid && control.status !== 'DISABLED') {
            if (control instanceof FormGroup || control instanceof FormArray) {
              const name = parentName + c + '.';
              list = [...list, ...this.listControlsWithErrors(control, ValidationFieldNameMap, name)];
            } else {
              let message = parentName + c + ' is not valid';
              if (ValidationFieldNameMap[parentName + c]) {
                message = ValidationFieldNameMap[parentName + c];
              }
              list.push(message);
            }
          }
        }
      }
    } else if (form instanceof FormArray) {
      form.controls.forEach((control, index) => {
        if (!control.valid && control.status !== 'DISABLED') {
          if (control instanceof FormGroup || control instanceof FormArray) {
            const name = parentName;
            list = [...list, ...this.listControlsWithErrors(control, ValidationFieldNameMap, name)];
          } else {
            let message = parentName + ' at index ' + index + 'is not valid';
            if (ValidationFieldNameMap[parentName]) {
              message = ValidationFieldNameMap[parentName] + ' at index ' + index;
            }
            list.push(message);
          }
        }
      });
    }
    return list;
  }


  markControlsAsTouched(form: FormGroup | FormArray) {
    if (form instanceof FormGroup) {
      for (const c in form.controls) {
        if (c) {
          const control = form.get(c);
          if (!control.valid) {
            if (control instanceof FormGroup || control instanceof FormArray) {
              this.markControlsAsTouched(control);
            } else {
              control.markAsTouched();
            }
          }
        }
      }
    } else if (form instanceof FormArray) {
      form.controls.forEach((control, index) => {
        if (!control.valid) {
          if (control instanceof FormGroup || control instanceof FormArray) {
            this.markControlsAsTouched(control);
          } else {
            control.markAsTouched();
          }
        }
      });
    }
  }

  getOptionFromLabel(options: any, label: string) {
    const idx = options.findIndex(opt => opt.label === label);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }

  getOptionFromValue(options: any, value: number) {
    const idx = options.findIndex(opt => opt.value === value);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }
}
