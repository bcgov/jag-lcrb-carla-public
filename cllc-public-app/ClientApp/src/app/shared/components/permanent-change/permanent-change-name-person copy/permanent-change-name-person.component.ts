import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The change name (person) section of a permanent change application.
 *
 * @export
 * @class PermanentChangeCannabisSecurityScreeningFormsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-name-person',
  templateUrl: './permanent-change-name-person.component.html',
  styleUrls: ['./permanent-change-name-person.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeNamePersonComponent implements OnInit {
  @Input() application: Application;

  @Output() uploadedNameChangeDocuments = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedNameChangeDocuments(event: number) {
    this.uploadedNameChangeDocuments.emit(event);
  }
}
