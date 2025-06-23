import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The disclaimer section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-disclaimer',
  templateUrl: './legal-entity-review-disclaimer.component.html',
  styleUrls: ['./legal-entity-review-disclaimer.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewDisclaimerComponent implements OnInit {
  @Input() account: Account;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
