import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Account } from '@models/account.model';
import { Subscription } from 'rxjs';

/**
 * Component for managing connections to other cannabis producers.
 *
 * @export
 * @class ConnectionToProducersComponent
 * @implements {OnInit}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-producers',
  templateUrl: './connection-to-producers.component.html',
  styleUrls: ['./connection-to-producers.component.scss']
})
export class ConnectionToProducersComponent implements OnInit, OnDestroy {
  @Input()
  account: Account;
  @Input()
  isMarketer: boolean;
  @Input()
  licensedProducerText = 'federally licensed producer';
  @Input()
  federalProducerText = 'federal producer';
  @Input()
  applicationTypeName: String;
  @Input()
  initialFormData: any;

  /**
   * Emits the form data on change.
   */
  @Output()
  onFormChanges = new EventEmitter();

  busy: Subscription;
  subscriptions: Subscription[] = [];
  savedFormData: any = {};

  form: any;

  constructor(
    private fb: FormBuilder,
    public snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      corpConnectionFederalProducer: [''],
      corpConnectionFederalProducerDetails: [''],
      federalProducerConnectionToCorp: [''],
      federalProducerConnectionToCorpDetails: [''],
      share20PlusConnectionProducer: [''],
      share20PlusConnectionProducerDetails: [''],
      share20PlusFamilyConnectionProducer: [''],
      share20PlusFamilyConnectionProducerDetail: [''],
      partnersConnectionFederalProducer: [''],
      partnersConnectionFederalProducerDetails: [''],
      societyConnectionFederalProducer: [''],
      societyConnectionFederalProducerDetails: [''],
      liquorFinancialInterest: [''],
      liquorFinancialInterestDetails: [''],
      iNConnectionToFederalProducer: [''],
      iNConnectionToFederalProducerDetails: ['']
    });

    if (this.initialFormData) {
      this.form.patchValue(this.initialFormData);
    }

    this.form.valueChanges.subscribe((value) => this.onFormChanges.emit(value));
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  /**
   * Checks if the form data has changed from the initial data.
   *
   * @return {*}  {boolean} `true` if the form data has changed, `false` otherwise.
   */
  formHasChanged(): boolean {
    if (JSON.stringify(this.initialFormData) !== JSON.stringify(this.form.value)) {
      return true;
    }

    return false;
  }

  requiresWordingChange(name: String): boolean {
    if (
      name === 'Producer Retail Store' ||
      name == 'PRS Relocation' ||
      name == 'PRS Transfer of Ownership' ||
      name == 'Section 119 Authorization(PRS)' ||
      name == 'CRS Renewal'
    ) {
      return true;
    }

    return false;
  }

  isPRS(): boolean {
    return this.applicationTypeName == 'Producer Retail Store';
  }
}
