import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Account } from '@models/account.model';
import { Application, ApplicationExtension } from '@models/application.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { isValidOrNotTouched } from '@shared/form-utils';
import { forkJoin, Subject, takeUntil } from 'rxjs';

export type ConnectionToOtherLiquorLicencesFormData = ApplicationExtension;

// Business rule: Tied House Connections are editable for certain licence types.
const LICENCE_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE = [
  'Agent',
  'Catering',
  'Food Primary',
  'Manufacturer',
  'Liquor Primary',
  'Liquor Primary Club',
  'UBrew and UVin',
  'Rural Licensee Retail Store'
];

// Business rule: Tied House Connections are editable for certain application types.
const APPLICATION_TYPES_FOR_WHICH_TIED_HOUSE_IS_EDITABLE = [
  'Third Party Operator',
  'Liquor Licence Renewal',
  'Liquor Licence Transfer'
];

/**
 * Component for managing connections to other liquor licences.
 * - Application level form fields pertaining to connections to other liquor licences
 * - Tied house connections to other liquor licences
 *
 * @export
 * @class ConnectionToOtherLiquorLicencesComponent
 * @implements {OnInit}
 * @implements {OnChanges}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-other-liquor-licences',
  templateUrl: './connection-to-other-liquor-licences.component.html',
  styleUrls: ['./connection-to-other-liquor-licences.component.scss']
})
export class ConnectionToOtherLiquorLicencesComponent implements OnInit, OnChanges, OnDestroy {
  /**
   * The user account information.
   */
  @Input() account: Account;
  /**
   * Optional application under which this component is being used.
   */
  @Input() application?: Application;
  /**
   * The initial form data to populate the form with.
   */
  @Input() initialFormData: ConnectionToOtherLiquorLicencesFormData;
  /**
   * Emits the form data on change.
   */
  @Output() onFormChanges = new EventEmitter<ConnectionToOtherLiquorLicencesFormData>();

  /**
   * The initial tied house data to populate the tied house declarations component with.
   */
  initialTiedHouseConnections: TiedHouseConnection[] = [];

  form: FormGroup;

  get accountId(): string | undefined {
    return this.account?.id;
  }

  get applicationId(): string | undefined {
    return this.application?.id;
  }

  hasLoadedData = false;

  destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.initFormInitialData();
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

    if (changes.initialFormData && !changes.initialFormData.firstChange && changes.initialFormData.currentValue) {
      // If the initial form data input changes, re-patch the form data.
      this.initFormInitialData();
    }
  }

  /**
   * Initialize the form.
   * This should only be called once.
   */
  initForm() {
    this.form = this.fb.group({
      hasLiquorTiedHouseOwnershipOrControl: [null, Validators.required],
      hasLiquorTiedHouseThirdPartyAssociations: [null, Validators.required],
      hasLiquorTiedHouseFamilyMemberInvolvement: [null, Validators.required]
    });

    this.form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => this.onFormChanges.emit(value));
  }

  /**
   * Initialize the form with initial data, if any is provided.
   */
  initFormInitialData() {
    if (!this.form) {
      return;
    }

    if (!this.initialFormData) {
      return;
    }

    this.form.patchValue(this.initialFormData);
  }

  /**
   * Set the form mode to either read-only or editable based on the current state.
   */
  setFormMode() {
    if (this.isTiedHouseReadOnly) {
      // If the tied house component is read-only, disable the form and disable all validators
      this.form.disable();
      this.form.get('hasLiquorTiedHouseOwnershipOrControl').clearValidators();
      this.form.get('hasLiquorTiedHouseThirdPartyAssociations').clearValidators();
      this.form.get('hasLiquorTiedHouseFamilyMemberInvolvement').clearValidators();
      this.form.updateValueAndValidity();
    } else {
      // If the tied house component is editable, enable the form and set all validators
      this.form.enable();
      this.form.get('hasLiquorTiedHouseOwnershipOrControl').setValidators([Validators.required]);
      this.form.get('hasLiquorTiedHouseThirdPartyAssociations').setValidators([Validators.required]);
      this.form.get('hasLiquorTiedHouseFamilyMemberInvolvement').setValidators([Validators.required]);
      this.form.updateValueAndValidity();
    }
  }

  /**
   * Fetch tied house data for the current account or application.
   */
  loadTiedHouseData() {
    if (!this.applicationId && !this.accountId) {
      return;
    }

    let tiedHouseConnectionsForApplicationIdRequest$ = this.applicationId
      ? this.tiedHouseService.GetLiquorTiedHouseConnectionsForApplication(this.applicationId)
      : this.tiedHouseService.GetLiquorTiedHouseConnectionsForUser(this.accountId);

    forkJoin({
      tiedHouseDataForApplication: tiedHouseConnectionsForApplicationIdRequest$
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ tiedHouseDataForApplication }) => {
          this.initialTiedHouseConnections = tiedHouseDataForApplication;

          this.hasLoadedData = true;
        },
        error: (error) => {
          console.error('Error loading Liquor Tied House data', error);
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
   * Indicates whether the tied house questions section should be shown.
   *
   * @readonly
   * @type {boolean}
   */
  get showTiedHouseQuestionsSection(): boolean {
    return !this.isTiedHouseReadOnly;
  }

  /**
   * Indicates whether the tied house declaration section should be shown.
   *
   * @readonly
   * @type {boolean}
   */
  get showTiedHouseDeclarationSection(): boolean {
    if (this.isTiedHouseReadOnly) {
      // Always show the section if the tied house component is read-only.
      return true;
    }

    if (!this.form) {
      return false;
    }

    if (this.initialTiedHouseConnections && this.initialTiedHouseConnections.length > 0) {
      // Show the section if the user has any existing Tied House connections
      return true;
    }

    if (
      this.form.get('hasLiquorTiedHouseOwnershipOrControl').value === 1 ||
      this.form.get('hasLiquorTiedHouseThirdPartyAssociations').value === 1 ||
      this.form.get('hasLiquorTiedHouseFamilyMemberInvolvement').value === 1
    ) {
      // Show the section if any of the tied house questions are answered with 'Yes'
      return true;
    }

    return false;
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
   * Checks if the form data has changed from the current data.
   *
   * @return {*}  {boolean} `true` if the form data has changed, `false` otherwise.
   */
  formHasChanged(): boolean {
    if (JSON.stringify(this.initialFormData) !== JSON.stringify({ ...this.initialFormData, ...this.form.value })) {
      return true;
    }

    return false;
  }

  /**
   * Checks if a form control is valid or not touched.
   *
   * @param {string} fieldName
   * @return {*}  {boolean}
   */
  isValidOrNotTouched(fieldName: string): boolean {
    if (this.isTiedHouseReadOnly) {
      // Mark form fields as valid, if the form is in a read-only state.
      return true;
    }

    return isValidOrNotTouched(this.form, fieldName);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
