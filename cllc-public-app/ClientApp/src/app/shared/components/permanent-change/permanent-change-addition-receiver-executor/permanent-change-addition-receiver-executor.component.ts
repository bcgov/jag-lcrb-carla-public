import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The addition of receiver or executor section of a permanent change application.
 *
 * @export
 * @class PermanentChangeAdditionReceiverExecutorComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-addition-receiver-executor',
  templateUrl: './permanent-change-addition-receiver-executor.component.html',
  styleUrls: ['./permanent-change-addition-receiver-executor.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeAdditionReceiverExecutorComponent implements OnInit {
  @Input() application: Application;

  @Output() uploadedExecutorDocuments = new EventEmitter<number>();
  @Output() uploadedlDeathCertificateDocuments = new EventEmitter<number>();
  @Output() uploadedletterOfIntentDocuments = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedExecutorDocuments(event: number) {
    this.uploadedExecutorDocuments.emit(event);
  }

  onUploadedlDeathCertificateDocuments(event: number) {
    this.uploadedlDeathCertificateDocuments.emit(event);
  }

  onUploadedletterOfIntentDocuments(event: number) {
    this.uploadedletterOfIntentDocuments.emit(event);
  }
}
