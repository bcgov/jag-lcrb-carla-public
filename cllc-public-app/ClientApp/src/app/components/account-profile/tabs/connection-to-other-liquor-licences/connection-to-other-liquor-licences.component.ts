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
  @Input() account: Account;

  @Output() onTiedHouseFormData = new EventEmitter<TiedHouseConnection>();

  tiedHouseFormData: TiedHouseConnection[];

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    public snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    // TODO: initialize the form checkbox values from the account data
    this.form = this.fb.group({
      hasOwnershipOrControl: [0],
      hasThirdPartyAssociations: [0],
      hasImmediateFamilyMemberInvolvement: [0]
    });
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
