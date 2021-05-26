import { EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { SepApplication } from '@models/sep-application.model';

@Component({
  selector: 'app-selling-drinks',
  templateUrl: './selling-drinks.component.html',
  styleUrls: ['./selling-drinks.component.scss']
})
export class SellingDrinksComponent implements OnInit {
  _application: SepApplication
  @Input()
  set application(value: SepApplication) {
    this._application = value;
    if (this.form) {
      this.form.patchValue(value);
    }
  };
  get application() {
    return this._application;
  }
  @Output() saved: EventEmitter<{ declaredServings: number }> = new EventEmitter<{ declaredServings: number }>();
  form: FormGroup;
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();
  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      chargingForLiquorReason: [''],
      isGSTRegisteredOrg: [''],
      donateOrConsularPrevLiqour: [''],
      nonProfitName: [''],
      fundraisingPurposeOfEvent: [''],
      wowProceedsWillBeUsedDescription: [''],
      isManufacturingExclusivity: ['']
    });
    if (this.application) {
      this.form.patchValue(this.application);
    }
  }

  next() {
    this.saved.next({ ...this.form.value });
  }
}
