import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';

/**
 * The supporting documents section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewSupportingDocumentsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-supporting-documents',
  templateUrl: './legal-entity-review-supporting-documents.component.html',
  styleUrls: ['./legal-entity-review-supporting-documents.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewSupportingDocumentsComponent implements OnInit {
  @Input() account: Account;
  @Input() application: Application;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  @Output() onUploadedFileEvent = new EventEmitter<number>();

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedFile(event: number) {
    this.onUploadedFileEvent.emit(event);
  }
}
