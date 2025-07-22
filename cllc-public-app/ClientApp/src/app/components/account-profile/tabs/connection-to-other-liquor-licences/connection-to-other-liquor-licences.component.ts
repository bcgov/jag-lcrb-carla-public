import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Account } from '@models/account.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { ApplicationDataService } from '@services/application-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';

/**
 * Component for managing connections to other liquor licences.
 *
 * @export
 * @class ConnectionToOtherLiquorLicencesComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-connection-to-other-liquor-licences',
  templateUrl: './connection-to-other-liquor-licences.component.html',
  styleUrls: ['./connection-to-other-liquor-licences.component.scss']
})
export class ConnectionToOtherLiquorLicencesComponent implements OnInit {
  /**
   * The user account information.
   */
  @Input() account: Account;
  /**
   * The application ID under which this component is being used.
   */
  @Input() applicationId?: string;
  /**
   * The initial form data to populate the form with.
   */
  @Input() initialFormData: any;
  /**
   * Emits the form data on change.
   */
  @Output() onFormChanges = new EventEmitter();

  initialTiedHouseConnections: TiedHouseConnection[] = [];

  userHasAtLeastOneApprovedApplication: boolean = false;
  userHasAtLeastOneExistingTiedHouseConnection: boolean = false;

  form: FormGroup;

  hasLoadedData = false;

  constructor(
    private fb: FormBuilder,
    private applicationDataService: ApplicationDataService,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    // TODO: tiedhouse - These need to be created in dynamics, and initialized with the initial form data, rather
    // than hardcoded to 0.
    this.form = this.fb.group({
      hasOwnershipOrControl: [0],
      hasThirdPartyAssociations: [0],
      hasImmediateFamilyMemberInvolvement: [0]
    });

    this.form.valueChanges.subscribe((value) => this.onFormChanges.emit(value));
  }

  loadData() {
    const tiedHouseConnectionsForApplicationIdRequest$ = this.tiedHouseService.GetAllTiedHouseConnectionsForApplication(
      this.applicationId
    );
    const tiedHouseConnectionsForUserRequest$ = this.tiedHouseService.GetAllTiedHouseConnectionsForUser();
    const applicationCountRequest$ = this.applicationDataService.getApprovedApplicationCount();

    forkJoin({
      tiedHouseDataForApplication: tiedHouseConnectionsForApplicationIdRequest$,
      tiedHouseDataForUser: tiedHouseConnectionsForUserRequest$,
      approvedApplicationCount: applicationCountRequest$
    }).subscribe({
      next: ({ tiedHouseDataForApplication, tiedHouseDataForUser, approvedApplicationCount }) => {
        this.initialTiedHouseConnections = [...tiedHouseDataForUser, ...tiedHouseDataForApplication];

        this.userHasAtLeastOneApprovedApplication = approvedApplicationCount > 0;
        this.userHasAtLeastOneExistingTiedHouseConnection = tiedHouseDataForUser.length > 0;

        this.hasLoadedData = true;
      },
      error: (error) => {
        console.error('Error loading Tied House data', error);
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
    if (this.applicationId !== null && this.applicationId !== undefined) {
      // If an application ID is provided then the form is editable.
      return false;
    }

    if (this.userHasAtLeastOneApprovedApplication || this.userHasAtLeastOneExistingTiedHouseConnection) {
      // If no application ID is provided, but the user also has no existing applications or tied house connections then
      // the form is editable.
      return false;
    }

    // The form is not editable.
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
   * Show the section if any of the checkboxes are selected.
   *
   * @readonly
   * @type {boolean}
   */
  get showTiedHouseDeclarationSection(): boolean {
    if (!this.form) {
      return false;
    }

    return (
      this.form.get('hasOwnershipOrControl').value === '1' ||
      this.form.get('hasThirdPartyAssociations').value === '1' ||
      this.form.get('hasImmediateFamilyMemberInvolvement').value === '1'
    );
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
}
