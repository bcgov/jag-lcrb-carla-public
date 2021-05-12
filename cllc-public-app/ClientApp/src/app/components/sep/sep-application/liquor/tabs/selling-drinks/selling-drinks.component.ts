import { EventEmitter, Output } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-selling-drinks',
  templateUrl: './selling-drinks.component.html',
  styleUrls: ['./selling-drinks.component.scss']
})
export class SellingDrinksComponent implements OnInit {
  @Output() saved: EventEmitter<{declaredServings: number}> = new EventEmitter<{declaredServings: number}>();
  form: FormGroup;
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();
  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      chargingForDrinks: [''],
      hostingAsAGSTOrg: [''],
      donateOrConsularPrevLiqour: [''],
      nameOfNonProfitOrg: [''],
      fundraisingPurposeOfEvent: [''],
      howWillProceedsBeUsed: [''],
      exclusivityAgreementWithAManufacturer: ['']
    });
  }

  next() {
    this.saved.next({...this.form.value});
  }
}
