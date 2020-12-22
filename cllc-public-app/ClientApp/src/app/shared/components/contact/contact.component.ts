import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';

export interface ContactData {
  contactPersonFirstName: string;
  contactPersonLastName: string;
  contactPersonRole: string;
  contactPersonPhone: string;
  contactPersonEmail: string;
}

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContactComponent extends FormBase implements OnInit {
  _disabled = false;
  _contact: ContactData = {} as ContactData;

  @Input()
  set disabled(val: boolean) {
    this._disabled = val;
    if (val && this.form) {
      this.form.disable();
    }
  }

  @Input()
  set contact(val: ContactData) {
    this._contact = val;
    if (this.form) {
      this.form.patchValue(val);
    }
  }
  get contact(): ContactData {
    return this._contact;
  }

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      contactPersonFirstName: ['', [Validators.required]],
      contactPersonLastName: ['', [Validators.required]],
      contactPersonRole: [''],
      contactPersonPhone: ['', [Validators.required]],
      contactPersonEmail: ['', [Validators.required]]
    });
    this.form.patchValue(this.contact);
    if (this._disabled) {
      this.form.disable();
    }
  }

}
