import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Account } from '@models/account.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';

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

  form: FormGroup;

  hasLoadedData = false;

  constructor(
    private fb: FormBuilder,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    // TODO: We need to initialize these 3 checkboxes from the account data? Do these fields exist in dynamics yet?
    this.form = this.fb.group({
      hasOwnershipOrControl: [0],
      hasThirdPartyAssociations: [0],
      hasImmediateFamilyMemberInvolvement: [0]
    });

    this.form.valueChanges.subscribe((value) => this.onFormChanges.emit(value));
  }

  loadData() {
    // If an application ID is provided, fetch tied house connections for that application.
    // Otherwise, fetch tied house connections for the current user.
    const request$ = this.applicationId
      ? this.tiedHouseService.GetAllTiedHouseConnectionsForApplication(this.applicationId)
      : this.tiedHouseService.GetAllTiedHouseConnectionsForUser();

    request$.subscribe({
      next: (data) => {
        this.initialTiedHouseConnections = data;

        this.hasLoadedData = true;
      },
      error: (error) => {
        console.error('Error loading Tied House data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Tied House Form Data',
            message:
              'Failed to load Tied House form data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
  }

  /**
   * Whether the Tied House Connections form is read-only.
   *
   * If this component is NOT being used in the context of an application (i.e., `applicationId` is not set), then
   * the form is read-only.
   *
   * @readonly
   * @type {boolean}
   */
  get isTiedHouseReadOnly(): boolean {
    return this.applicationId === null || this.applicationId === undefined;
  }

  /**
   * Indicates whether there are any tied house connections.
   *
   * @readonly
   * @type {boolean}
   */
  get hasTiedHouseConnections(): boolean {
    return this.initialTiedHouseConnections?.length > 0;
  }

  /**
   * Indicates whether the Tied House checkboxes should be shown, which allows users to toggle the tied house
   * declaration section, in order to add/edit tied house connections.
   *
   * @readonly
   * @type {boolean}
   */
  get showTiedHouseCheckboxes(): boolean {
    if (this.isTiedHouseReadOnly) {
      // If the form is read-only, do not show the checkboxes, as the user cannot add/edit tied house connections.
      return false;
    }

    if (this.hasTiedHouseConnections) {
      // If the user has existing tied house connections, do not show the checkboxes.
      return false;
    }

    return true;
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
