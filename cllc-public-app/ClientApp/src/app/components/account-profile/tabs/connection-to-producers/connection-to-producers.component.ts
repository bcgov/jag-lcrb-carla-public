import { Component, OnInit, Input, OnDestroy, Output, EventEmitter } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { FormBuilder } from "@angular/forms";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { Subscription } from "rxjs";
import { Account } from "@models/account.model";
import { ApplicationType } from "@models/application-type.model";

@Component({
  selector: "app-connection-to-producers",
  templateUrl: "./connection-to-producers.component.html",
  styleUrls: ["./connection-to-producers.component.css"]
})
export class ConnectionToProducersComponent implements OnInit, OnDestroy {
  @Input()
  account: Account;
  @Input()
  isMarketer: boolean;
  @Input()
  licensedProducerText = "federally licensed producer";
  @Input()
  federalProducerText = "federal producer";
  @Input()
  applicationTypeName: String;

  @Input("tiedHouse")
  set tiedHouse(value: TiedHouseConnection) {
    if (value && this.form) {
      this.form.patchValue(value);
    }
    this._tiedHouseData = value;
  }

  get tiedHouse(): TiedHouseConnection {
    return { ...this._tiedHouseData };
  }

  busy: Subscription;
  subscriptions: Subscription[] = [];
  savedFormData: any = {};

  form: any;
  _tiedHouseData: TiedHouseConnection;
  @Output()
  value = new EventEmitter<TiedHouseConnection>();

  constructor(private fb: FormBuilder,
    public snackBar: MatSnackBar) {
  }

  ngOnInit() {
    this.form = this.fb.group({
      corpConnectionFederalProducer: [""],
      corpConnectionFederalProducerDetails: [""],
      federalProducerConnectionToCorp: [""],
      federalProducerConnectionToCorpDetails: [""],
      share20PlusConnectionProducer: [""],
      share20PlusConnectionProducerDetails: [""],
      share20PlusFamilyConnectionProducer: [""],
      share20PlusFamilyConnectionProducerDetail: [""],
      partnersConnectionFederalProducer: [""],
      partnersConnectionFederalProducerDetails: [""],
      societyConnectionFederalProducer: [""],
      societyConnectionFederalProducerDetails: [""],
      liquorFinancialInterest: [""],
      liquorFinancialInterestDetails: [""],
      iNConnectionToFederalProducer: [""],
      iNConnectionToFederalProducerDetails: [""],
    });

    if (this.tiedHouse) {
      this.form.patchValue(this.tiedHouse);
    }
    this.form.valueChanges.subscribe(value => this.value.emit(Object.assign(this.tiedHouse, value)));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  formHasChanged(): boolean {
    let hasChanged = false;
    const data = (Object as any).assign(this.tiedHouse, this.form.value);
    if (JSON.stringify(data) !== JSON.stringify(this.tiedHouse)) {
      hasChanged = true;
    }
    return hasChanged;
  }

  requiresWordingChange(name: String): boolean {
    if (name === "Producer Retail Store" ||
      name == "PRS Relocation" || name == "PRS Transfer of Ownership" || name == "Section 119 Authorization(PRS)" || name == "CRS Renewal") {
      return true;
    }

    return false;
  }
}
