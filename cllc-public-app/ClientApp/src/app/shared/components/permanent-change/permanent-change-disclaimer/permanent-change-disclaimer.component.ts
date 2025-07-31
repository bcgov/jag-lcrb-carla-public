import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The disclaimer section of a permanent change application.
 *
 * @export
 * @class PermanentChangeDisclaimerComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-disclaimer',
  templateUrl: './permanent-change-disclaimer.component.html',
  styleUrls: ['./permanent-change-disclaimer.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeDisclaimerComponent implements OnInit {
  @Input() account: Account;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
