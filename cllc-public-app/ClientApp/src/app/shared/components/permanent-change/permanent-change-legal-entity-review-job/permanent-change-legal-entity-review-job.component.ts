import { ViewportScroller } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The legal entity review job section of a permanent change legal entity review application.
 *
 * @export
 * @class PermanentChangeLegalEntityReviewJobComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-legal-entity-review-job',
  templateUrl: './permanent-change-legal-entity-review-job.component.html',
  styleUrls: ['./permanent-change-legal-entity-review-job.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeLegalEntityReviewJobComponent implements OnInit {
  @Input() account: Account;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(
    public controlContainer: ControlContainer,
    private scroller: ViewportScroller
  ) {}

  scrollToQuestionsSection() {
    this.scroller.scrollToAnchor('questionssection');
  }

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
