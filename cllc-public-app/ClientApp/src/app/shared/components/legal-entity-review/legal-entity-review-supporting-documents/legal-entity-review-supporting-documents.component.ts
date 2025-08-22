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

  onUploadedFileCount(event: number) {
    this.onUploadedFileCountEvent.emit(event);
  }
}
