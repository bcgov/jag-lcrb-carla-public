import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormBuilder, FormGroupDirective, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';

export interface ContactData {
  contactPersonFirstName: string;
  contactPersonLastName: string;
  contactPersonRole: string;
  contactPersonPhone: string;
  contactPersonEmail: string;
}

@Component({
  selector: 'app-permanent-change-contact',
  templateUrl: './permanent-change-contact.component.html',
  styleUrls: ['./permanent-change-contact.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeContactComponent extends FormBase implements OnInit {
  _disabled = false;
  _contact = {} as ContactData;

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
