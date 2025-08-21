import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The change name, licensee (socitey) section of a permanent change application.
 *
 * @export
 * @class PermanentChangeNameLicenseeSocitey
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-name-licensee-society',
  templateUrl: './permanent-change-name-licensee-society.component.html',
  styleUrls: ['./permanent-change-name-licensee-society.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeNameLicenseeSocitey implements OnInit {
  @Input() application: Application;
  @Input() disabled: boolean = false;

  @Output() uploadedSocietyNameChange = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedSocietyNameChange(event: number) {
    this.uploadedSocietyNameChange.emit(event);
  }
}
