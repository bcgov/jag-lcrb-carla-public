import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The what happens next section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewWhatHappensNextComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-what-happens-next',
  templateUrl: './legal-entity-review-what-happens-next.component.html',
  styleUrls: ['./legal-entity-review-what-happens-next.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewWhatHappensNextComponent implements OnInit {
  @Input() account: Account;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
