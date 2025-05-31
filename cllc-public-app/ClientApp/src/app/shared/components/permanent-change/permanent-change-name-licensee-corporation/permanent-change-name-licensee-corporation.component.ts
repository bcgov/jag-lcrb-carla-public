import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The change name, licensee (corporation) section of a permanent change application.
 *
 * @export
 * @class PermanentChangeNameLicenseeCorporationComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-name-licensee-corporation',
  templateUrl: './permanent-change-name-licensee-corporation.component.html',
  styleUrls: ['./permanent-change-name-licensee-corporation.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeNameLicenseeCorporationComponent implements OnInit {
  @Input() application: Application;
  @Input() showValidationMessages: boolean = false;

  @Output() uploadedCertificateOfNameChange = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedCertificateOfNameChange(event: number) {
    this.uploadedCertificateOfNameChange.emit(event);
  }
}
