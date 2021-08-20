import { EventEmitter, Input, Output } from "@angular/core";
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { SepApplication } from "@models/sep-application.model";
import { FormBase } from "@shared/form-base";

@Component({
  selector: "app-selling-drinks",
  templateUrl: "./selling-drinks.component.html",
  styleUrls: ["./selling-drinks.component.scss"]
})
export class SellingDrinksComponent extends FormBase implements OnInit {
  _application: SepApplication;
  errorMessages: string[];
  @Input()
  set sepApplication(value: SepApplication) {
    this._application = value;
    if (this.form) {
      this.form.patchValue(value);
    }
  }
  get sepApplication() {
    return this._application;
  }
  @Output() saved: EventEmitter<{ declaredServings: number }> = new EventEmitter<{ declaredServings: number }>();
  form: FormGroup;
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();
  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      chargingForLiquorReason: [this?.sepApplication?.chargingForLiquorReason, [Validators.required]],
      isGSTRegisteredOrg: [this?.sepApplication?.isGSTRegisteredOrg, [Validators.required]],
      donateOrConsular: [this?.sepApplication?.donateOrConsular, [Validators.required]],
      nonProfitName: [this?.sepApplication?.nonProfitName],
      // fundraisingPurpose: [this?.application?.fundraisingPurpose],
      // howProceedsWillBeUsedDescription: [this?.application?.howProceedsWillBeUsedDescription],
      isManufacturingExclusivity: [this?.sepApplication?.isManufacturingExclusivity]
    });

    this.form.get("chargingForLiquorReason").valueChanges
      .subscribe(reason => {
        if (reason === "RaiseMoney") {
          this.form.get("nonProfitName").setValidators(Validators.required);
        } else {
          this.form.get("nonProfitName").clearValidators();
          this.form.get("nonProfitName").updateValueAndValidity();
        }
      });

  }

  isValid(): boolean {
    this.markControlsAsTouched(this.form);
    this.errorMessages = this.listControlsWithErrors(this.form, {
      // chargingForLiquorReason: "",
      // isGSTRegisteredOrg: "",
      // donateOrConsularPrevLiqour: "",
      // nonProfitName: "",
      // isManufacturingExclusivity: "",
    });
    const valid = this.form.valid;
    return valid;
  }

  next() {
    if (this.isValid()) {
      this.saved.next({ ...this.form.value });
    }
  }
}
