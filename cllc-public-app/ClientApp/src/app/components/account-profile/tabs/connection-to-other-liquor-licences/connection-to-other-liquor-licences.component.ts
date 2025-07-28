import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Account } from '@models/account.model';
import { ApplicationExtension } from '@models/application.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { ApplicationDataService } from '@services/application-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { of, Subject, takeUntil } from 'rxjs';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';

export type ConnectionToOtherLiquorLicencesFormData = ApplicationExtension;

/**
 * Component for managing connections to other liquor licences.
 * - Application level form fields pertaining to connections to other liquor licences
 * - Tied house connections to other liquor licences
 *
 * @export
 * @class ConnectionToOtherLiquorLicencesComponent
 * @implements {OnInit}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-other-liquor-licences',
  templateUrl: './connection-to-other-liquor-licences.component.html',
  styleUrls: ['./connection-to-other-liquor-licences.component.scss']
})
export class ConnectionToOtherLiquorLicencesComponent implements OnInit, OnDestroy {
  /**
   * The user account information.
   */
  @Input() account: Account;
  /**
   * Optional application ID under which this component is being used.
   */
  @Input() applicationId?: string;
  /**
   * The initial form data to populate the form with.
   */
  @Input() initialFormData: ConnectionToOtherLiquorLicencesFormData;
  /**
   * Emits the form data on change.
   */
  @Input() readonly = false;
  @Output() onFormChanges = new EventEmitter<ConnectionToOtherLiquorLicencesFormData>();

  /**
   * The initial tied house data to populate the tied house declarations component with.
   */
  initialTiedHouseConnections: TiedHouseConnection[] = [];

  userHasAtLeastOneApprovedApplication: boolean = false;
  userHasAtLeastOneExistingTiedHouseConnection: boolean = false;

  form: FormGroup;

  get accountId(): string | undefined {
    return this.account?.id;
  }

  get busy(): boolean {
    return !this.hasLoadedData;
  }

  hasLoadedData = false;

  destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private applicationDataService: ApplicationDataService,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadFormData();
    this.loadTiedHouseData();
    if (this.readonly) {
      this.form.disable();
    }
  }

  initForm() {
    this.form = this.fb.group({
      hasLiquorTiedHouseOwnershipOrControl: [''],
      hasLiquorTiedHouseThirdPartyAssociations: [''],
      hasLiquorTiedHouseFamilyMemberInvolvement: ['']
    });

    this.form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => this.onFormChanges.emit(value));
  }

  loadFormData() {
    if (this.initialFormData) {
      this.form.patchValue(this.initialFormData);
    }
  }

  loadTiedHouseData() {
    const tiedHouseConnectionsForApplicationIdRequest$ = this.applicationId
      ? this.tiedHouseService.GetAllLiquorTiedHouseConnectionsForApplication(this.applicationId)
      : of([]);

    const tiedHouseConnectionsForUserRequest$ = this.tiedHouseService.GetAllLiquorTiedHouseConnectionsForUser();
    const applicationCountRequest$ = this.applicationDataService.getApprovedApplicationCount();

    forkJoin({
      tiedHouseDataForApplication: tiedHouseConnectionsForApplicationIdRequest$,
      tiedHouseDataForUser: tiedHouseConnectionsForUserRequest$,
      approvedApplicationCount: applicationCountRequest$
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ tiedHouseDataForApplication, tiedHouseDataForUser, approvedApplicationCount }) => {
          this.initialTiedHouseConnections = [...tiedHouseDataForUser, ...tiedHouseDataForApplication];

          this.userHasAtLeastOneApprovedApplication = approvedApplicationCount > 0;
          this.userHasAtLeastOneExistingTiedHouseConnection = tiedHouseDataForUser.length > 0;

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
   * Whether the Tied House Connections form is read-only.
   *
   * @readonly
   * @type {boolean}
   */
  get isTiedHouseReadOnly(): boolean {
    if (this.readonly) {
      return true;
    }
    if (this.applicationId !== null && this.applicationId !== undefined) {
      // If an application ID is provided then the form is editable.
      console.log('not readonly 1');
      return false;
    }

    if (!this.userHasAtLeastOneApprovedApplication && !this.userHasAtLeastOneExistingTiedHouseConnection) {
      // If no application ID is provided, but the user also has no existing applications and no existing tied house
      // connections then the form is editable.
      return false;
    }

    // The form is read-only (not editable).
    return true;
  }

  /**
   * Whether there are any initial tied house connections.
   *
   * @readonly
   * @type {boolean}
   */
  get hasInitialTiedHouseConnections(): boolean {
    return this.initialTiedHouseConnections?.length > 0;
  }

  /**
   * Indicates whether the tied house declaration section should be shown.
   *
   * @readonly
   * @type {boolean}
   */
  get showTiedHouseDeclarationSection(): boolean {
    if (!this.form) {
      return false;
    }

    if (this.isTiedHouseReadOnly) {
      return true;
    }

    if (
      this.form.get('hasLiquorTiedHouseOwnershipOrControl').value === 1 ||
      this.form.get('hasLiquorTiedHouseThirdPartyAssociations').value === 1 ||
      this.form.get('hasLiquorTiedHouseFamilyMemberInvolvement').value === 1
    ) {
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
