import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Account } from '@models/account.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';

@Component({
  selector: 'app-connection-to-other-liquor-licences',
  templateUrl: './connection-to-other-liquor-licences.component.html',
  styleUrls: ['./connection-to-other-liquor-licences.component.scss']
})
export class ConnectionToOtherLiquorLicencesComponent implements OnInit {
  /**
   * The user account information.
   *
   * @type {Account}
   */
  @Input() account: Account;
  /**
   * The application ID under which this component is being used.
   *
   * @type {string}
   */
  @Input() applicationId?: string;

  @Output() onTiedHouseFormData = new EventEmitter<TiedHouseConnection>();

  tiedHouseFormData: TiedHouseConnection[] = [];

  form: FormGroup;

  get tiedHouseConnections(): TiedHouseConnection[] {
    return this.account?.tiedHouse ?? [];
  }

  constructor(
    private fb: FormBuilder,
    public snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    // TODO: We need to initialize these 3 checkboxes from the account data? Do these fields exist in dynamics yet?
    this.form = this.fb.group({
      hasOwnershipOrControl: [0],
      hasThirdPartyAssociations: [0],
      hasImmediateFamilyMemberInvolvement: [0]
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
   * Checks if the form data has changed compared to the initial tied house form data this component was initialized
   * with.
   *
   * @return {*}  {boolean}
   */
  formHasChanged(): boolean {
    let hasChanged = false;

    const data = (Object as any).assign(this.tiedHouseFormData, this.form.value);

    if (JSON.stringify(data) !== JSON.stringify(this.tiedHouseFormData)) {
      hasChanged = true;
    }

    return hasChanged;
  }
}
