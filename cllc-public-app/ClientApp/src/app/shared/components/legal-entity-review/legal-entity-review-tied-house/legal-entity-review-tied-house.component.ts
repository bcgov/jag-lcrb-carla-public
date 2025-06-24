import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The tied house connections section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-tied-house',
  templateUrl: './legal-entity-review-tied-house.component.html',
  styleUrls: ['./legal-entity-review-tied-house.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewTiedHouseComponent implements OnInit {
  @Input() account: Account;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  hasTiedHouseChangesToDeclare = false;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
