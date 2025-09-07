import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { Subject, takeUntil } from 'rxjs';

export type ConnectionToProducersFormData = TiedHouseConnection;

// Business rule: Tied House Connections are editable for certain licence types.
const LICENCE_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE = [];

// Business rule: Tied House Connections are editable for certain application types.
const APPLICATION_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE = [
  'CRS Renewal',
  'Marketing Renewal',
  'CRS Transfer of Ownership',
  'PRS Transfer of Ownership'
];

/**
 * Component for managing connections to other cannabis producers.
 * - A singleton tied house connection for connections to cannabis producers
 *
 * @export
 * @class ConnectionToProducersComponent
 * @implements {OnInit}
 * @implements {OnChanges}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-producers',
  templateUrl: './connection-to-producers.component.html',
  styleUrls: ['./connection-to-producers.component.scss']
})
export class ConnectionToProducersComponent implements OnInit, OnChanges, OnDestroy {
  /**
   * The user account information.
   */
  @Input() account: Account;
  /**
   * Optional application under which this component is being used.
   */
  @Input() application: Application;
  /**
   * Emits the form data on change.
   */
  @Output() onFormChanges = new EventEmitter();

  /**
   * The initial tied house data to populate the tied house declarations component with.
   */
  initialTiedHouseConnection: TiedHouseConnection;

  form: FormGroup;

  get busy(): boolean {
    return !this.hasLoadedData;
  }

  hasLoadedData = false;

  destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private tiedHouseService: TiedHouseConnectionsDataService,
    public snackBar: MatSnackBar,
    public matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadTiedHouseData();
    this.setFormMode();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes.account &&
        !changes.account.firstChange &&
        changes.account.currentValue !== changes.account.previousValue) ||
      (changes.application &&
        !changes.application.firstChange &&
        changes.application.currentValue !== changes.application.previousValue)
    ) {
      // If the account or application input changes, re-load the tiedhouse data and update the form mode.
      this.loadTiedHouseData();
      this.setFormMode();
    }
  }

  initForm() {
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

    this.form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => this.onFormChanges.emit(value));
  }

  /**
   * Set the form mode to either read-only or editable based on the current state.
   */
  setFormMode() {
    if (this.isTiedHouseReadOnly) {
      // If the tied house component is read-only, disable the form and disable all validators
      this.form.disable();
      this.form.get('corpConnectionFederalProducer').clearValidators();
      this.form.get('corpConnectionFederalProducerDetails').clearValidators();
      this.form.get('federalProducerConnectionToCorp').clearValidators();
      this.form.get('federalProducerConnectionToCorpDetails').clearValidators();
      this.form.get('share20PlusConnectionProducer').clearValidators();
      this.form.get('share20PlusConnectionProducerDetails').clearValidators();
      this.form.get('share20PlusFamilyConnectionProducer').clearValidators();
      this.form.get('share20PlusFamilyConnectionProducerDetail').clearValidators();
      this.form.get('partnersConnectionFederalProducer').clearValidators();
      this.form.get('partnersConnectionFederalProducerDetails').clearValidators();
      this.form.get('societyConnectionFederalProducer').clearValidators();
      this.form.get('societyConnectionFederalProducerDetails').clearValidators();
      this.form.get('liquorFinancialInterest').clearValidators();
      this.form.get('liquorFinancialInterestDetails').clearValidators();
      this.form.get('iNConnectionToFederalProducer').clearValidators();
      this.form.get('iNConnectionToFederalProducerDetails').clearValidators();
      this.form.updateValueAndValidity();
    } else {
      // If the tied house component is editable, enable the form and set all validators
      this.form.enable();
      this.form.get('corpConnectionFederalProducer').setValidators([Validators.required]);
      this.form.get('corpConnectionFederalProducerDetails').setValidators([Validators.required]);
      this.form.get('federalProducerConnectionToCorp').setValidators([Validators.required]);
      this.form.get('federalProducerConnectionToCorpDetails').setValidators([Validators.required]);
      this.form.get('share20PlusConnectionProducer').setValidators([Validators.required]);
      this.form.get('share20PlusConnectionProducerDetails').setValidators([Validators.required]);
      this.form.get('share20PlusFamilyConnectionProducer').setValidators([Validators.required]);
      this.form.get('share20PlusFamilyConnectionProducerDetail').setValidators([Validators.required]);
      this.form.get('partnersConnectionFederalProducer').setValidators([Validators.required]);
      this.form.get('partnersConnectionFederalProducerDetails').setValidators([Validators.required]);
      this.form.get('societyConnectionFederalProducer').setValidators([Validators.required]);
      this.form.get('societyConnectionFederalProducerDetails').setValidators([Validators.required]);
      this.form.get('liquorFinancialInterest').setValidators([Validators.required]);
      this.form.get('liquorFinancialInterestDetails').setValidators([Validators.required]);
      this.form.get('iNConnectionToFederalProducer').setValidators([Validators.required]);
      this.form.get('iNConnectionToFederalProducerDetails').setValidators([Validators.required]);
      this.form.updateValueAndValidity();
    }
  }

  loadTiedHouseData() {
    const cannabisTiedHouseConnectionForUserRequest$ = this.tiedHouseService.GetCannabisTiedHouseConnectionForUser(
      this.account.id
    );

    cannabisTiedHouseConnectionForUserRequest$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (cannabisTiedHouseDataForUser) => {
        this.initialTiedHouseConnection = cannabisTiedHouseDataForUser;

        if (this.initialTiedHouseConnection) {
          this.form.patchValue(this.initialTiedHouseConnection);
        }

        // Register change handlers to clear the details field when the corresponding checkbox field is unchecked
        this.clearDetailsWhenCheckboxIsFalse('corpConnectionFederalProducer', 'corpConnectionFederalProducerDetails');
        this.clearDetailsWhenCheckboxIsFalse(
          'federalProducerConnectionToCorp',
          'federalProducerConnectionToCorpDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse('share20PlusConnectionProducer', 'share20PlusConnectionProducerDetails');
        this.clearDetailsWhenCheckboxIsFalse(
          'share20PlusFamilyConnectionProducer',
          'share20PlusFamilyConnectionProducerDetail'
        );
        this.clearDetailsWhenCheckboxIsFalse(
          'partnersConnectionFederalProducer',
          'partnersConnectionFederalProducerDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse(
          'societyConnectionFederalProducer',
          'societyConnectionFederalProducerDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse('liquorFinancialInterest', 'liquorFinancialInterestDetails');
        this.clearDetailsWhenCheckboxIsFalse('iNConnectionToFederalProducer', 'iNConnectionToFederalProducerDetails');

        this.hasLoadedData = true;
      },
      error: (error) => {
        console.error('Error loading Cannabis Tied House data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Tied House Data',
            message:
              'Failed to load Tied House data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
  }

  /**
   * Whether the Tied House Connections component is read-only.
   *
   * @readonly
   * @type {boolean}
   */
  get isTiedHouseReadOnly(): boolean {
    if (this.application) {
      // If an application is provided, then the tied house component is editable if the application is of a certain type.
      return !this.isTiedHouseConnectionsEditableForApplication;
    }

    return true;
  }

  /**
   * Checks if the tied house declarations are editable based on the type of application.
   *
   * @readonly
   * @type {boolean}
   */
  get isTiedHouseConnectionsEditableForApplication(): boolean {
    if (!this.application) {
      // If no application is provided, the tied house component is not editable.
      return false;
    }

    if (LICENCE_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE.includes(this.application.licenseType)) {
      // If the application is for a licence of a certain type, then the tied house component is editable.
      return true;
    }

    if (APPLICATION_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE.includes(this.application.applicationType.name)) {
      // If the application is of a certain type, then the tied house component is editable.
      return true;
    }

    return false;
  }

  /**
   * Change handler to clear the details field when the corresponding checkbox field is unchecked.
   *
   * @param {string} checkboxFormControlName
   * @param {string} detailsFormControlName
   */
  clearDetailsWhenCheckboxIsFalse(checkboxFormControlName: string, detailsFormControlName: string) {
    this.form
      .get(checkboxFormControlName)
      ?.valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((value) => {
        if (value === 0) {
          this.form.get(detailsFormControlName)?.setValue('');
        }
      });
  }

  /**
   * Gets the page header text based on the application type.
   *
   * @return {*}  {string}
   */
  get pageHeader(): string {
    const applicationTypeName = this.application?.applicationType?.name;
    if (
      applicationTypeName === 'Producer Retail Store' ||
      applicationTypeName == 'PRS Relocation' ||
      applicationTypeName == 'PRS Transfer of Ownership' ||
      applicationTypeName == 'Section 119 Authorization(PRS)' ||
      applicationTypeName == 'CRS Renewal'
    ) {
      return 'CONNECTIONS TO OTHER FEDERAL PRODUCERS';
    }

    return 'CONNECTIONS TO FEDERAL PRODUCERS OF CANNABIS';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
