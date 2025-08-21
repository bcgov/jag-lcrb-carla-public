import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The change name, licensee (partnership) section of a permanent change application.
 *
 * @export
 * @class PermanentChangeNameLicenseePartnership
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-name-licensee-partnership',
  templateUrl: './permanent-change-name-licensee-partnership.component.html',
  styleUrls: ['./permanent-change-name-licensee-partnership.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeNameLicenseePartnership implements OnInit {
  @Input() application: Application;
  @Input() disabled: boolean = false;

  @Output() uploadedPartnershipRegistration = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedPartnershipRegistration(event: number) {
    this.uploadedPartnershipRegistration.emit(event);
  }
}
