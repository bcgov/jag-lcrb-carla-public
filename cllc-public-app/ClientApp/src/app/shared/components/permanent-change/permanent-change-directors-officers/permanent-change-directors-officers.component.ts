import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The cannabis associate security screening forms section of a permanent change application.
 *
 * @export
 * @class PermanentChangeDirectorsOfficers
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-directors-officers',
  templateUrl: './permanent-change-directors-officers.component.html',
  styleUrls: ['./permanent-change-directors-officers.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeDirectorsOfficers implements OnInit {
  @Input() hasLiquor: boolean = false;
  @Input() hasCannabis: boolean = false;
  @Input() application: Application;

  @Output() uploadedNOA = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedNOA(event: number) {
    this.uploadedNOA.emit(event);
  }
}
