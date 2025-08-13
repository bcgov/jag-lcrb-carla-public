import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';

/**
 * The legal entity review job section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewJobComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-job',
  templateUrl: './legal-entity-review-job.component.html',
  styleUrls: ['./legal-entity-review-job.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewJobComponent implements OnInit {
  @Input() account: Account;
  @Input() application: Application;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
