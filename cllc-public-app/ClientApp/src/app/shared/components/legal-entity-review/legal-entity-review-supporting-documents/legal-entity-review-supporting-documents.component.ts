import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Application } from '@models/application.model';

/**
 * The supporting documents section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewSupportingDocumentsComponent
 */
@Component({
  selector: 'app-legal-entity-review-supporting-documents',
  templateUrl: './legal-entity-review-supporting-documents.component.html',
  styleUrls: ['./legal-entity-review-supporting-documents.component.scss']
})
export class LegalEntityReviewSupportingDocumentsComponent {
  @Input() application: Application;
  @Input() disabled = false;
  @Input() required = false;

  @Output() onUploadedFileCountEvent = new EventEmitter<number>();

  form: FormGroup;

  readonly requiredHeader = 'Add Required Information/Documents';
  readonly optionalHeader = 'Add Supporting Information/Documents (optional)';

  readonly requiredText =
    'Use the upload box to provide the information requested in the letter above. Refer to your Legal Entity Review letter for a full list of required information/documents to submit.';
  readonly optionalText = 'Use the upload box to provide any supporting information/documents.';

  onUploadedFileCount(event: number) {
    this.onUploadedFileCountEvent.emit(event);
  }

  get sectionHeader(): string {
    return this.required ? this.requiredHeader : this.optionalHeader;
  }

  get sectionText(): string {
    return this.required ? this.requiredText : this.optionalText;
  }
}
