import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The internal transfer of shares section of a permanent change application.
 *
 * @export
 * @class PermanentChangeInternalTransferShares
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-internal-transfer-shares',
  templateUrl: './permanent-change-internal-transfer-shares.component.html',
  styleUrls: ['./permanent-change-internal-transfer-shares.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeInternalTransferShares implements OnInit {
  @Input() application: Application;

  @Output() uploadedCentralSecuritiesRegister = new EventEmitter<number>();
  @Output() uploadedIndividualsWithLessThan10 = new EventEmitter<number>();
  @Output() uploadedCertificateOfAmalgamation = new EventEmitter<number>();
  @Output() uploadedNOAAmalgamation = new EventEmitter<number>();

  faIdCard = faIdCard;

  isAmalgamated = false;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedCentralSecuritiesRegister(event: number) {
    this.uploadedCentralSecuritiesRegister.emit(event);
  }

  onUploadedIndividualsWithLessThan10(event: number) {
    this.uploadedIndividualsWithLessThan10.emit(event);
  }

  onUploadedCertificateOfAmalgamation(event: number) {
    this.uploadedCertificateOfAmalgamation.emit(event);
  }

  onUploadedNOAAmalgamation(event: number) {
    this.uploadedNOAAmalgamation.emit(event);
  }
}
