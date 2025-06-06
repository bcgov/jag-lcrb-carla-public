import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The external transfer of shares section of a permanent change application.
 *
 * @export
 * @class PermanentChangeExternalTransferShares
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-external-transfer-shares',
  templateUrl: './permanent-change-external-transfer-shares.component.html',
  styleUrls: ['./permanent-change-external-transfer-shares.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeExternalTransferShares implements OnInit {
  @Input() application: Application;

  @Output() uploadedCentralSecuritiesRegister = new EventEmitter<number>();
  @Output() uploadedRegisterOfDirectorsAndOfficers = new EventEmitter<number>();
  @Output() uploadedIndividualsWithLessThan10 = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedCentralSecuritiesRegister(event: number) {
    this.uploadedCentralSecuritiesRegister.emit(event);
  }

  onUploadedRegisterOfDirectorsAndOfficers(event: number) {
    this.uploadedRegisterOfDirectorsAndOfficers.emit(event);
  }

  onUploadedIndividualsWithLessThan10(event: number) {
    this.uploadedIndividualsWithLessThan10.emit(event);
  }
}
