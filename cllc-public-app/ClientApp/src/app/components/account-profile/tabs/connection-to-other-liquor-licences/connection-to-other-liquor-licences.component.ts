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
  @Input() isMarketer: boolean;
  @Input() licensedProducerText = 'federally licensed producer';
  @Input() federalProducerText = 'federal producer';
  @Input() applicationTypeName: String;

  @Input('tiedHouse')
  set tiedHouse(value: TiedHouseConnection) {
    if (value && this.form) {
      this.form.patchValue(value);
    }

    this._tiedHouseData = value;
  }

  get tiedHouse(): TiedHouseConnection {
    return { ...this._tiedHouseData };
  }

  @Output() value = new EventEmitter<TiedHouseConnection>();

  _tiedHouseData: TiedHouseConnection;

  form: FormGroup | undefined;

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

  constructor(
    private fb: FormBuilder,
    public snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      hasOwnershipOrControl: [''],
      hasThirdPartyAssociations: [''],
      hasImmediateFamilyMemberInvolvement: ['']
    });

    if (this.tiedHouse) {
      this.form.patchValue(this.tiedHouse);
    }

    this.form.valueChanges.subscribe((value) => this.value.emit(Object.assign(this.tiedHouse, value)));
  }

  /**
   * Checks if the form data has changed compared to the initial tied house form data this component was initialized
   * with.
   *
   * @return {*}  {boolean}
   */
  formHasChanged(): boolean {
    let hasChanged = false;

    const data = (Object as any).assign(this.tiedHouse, this.form.value);

    if (JSON.stringify(data) !== JSON.stringify(this.tiedHouse)) {
      hasChanged = true;
    }

    return hasChanged;
  }
}
