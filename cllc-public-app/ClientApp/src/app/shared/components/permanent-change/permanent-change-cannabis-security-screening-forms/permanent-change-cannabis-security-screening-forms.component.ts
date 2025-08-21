import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The cannabis associate security screening forms section of a permanent change application.
 *
 * @export
 * @class PermanentChangeCannabisSecurityScreeningFormsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-cannabis-security-screening-forms',
  templateUrl: './permanent-change-cannabis-security-screening-forms.component.html',
  styleUrls: ['./permanent-change-cannabis-security-screening-forms.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeCannabisSecurityScreeningFormsComponent implements OnInit {
  @Input() application: Application;
  @Input() disabled: boolean = false;

  @Output() uploadedCAS = new EventEmitter<number>();
  @Output() uploadedFinancialIntegrity = new EventEmitter<number>();

  faIdCard = faIdCard;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedCAS(event: number) {
    this.uploadedCAS.emit(event);
  }

  onUploadedFinancialIntegrity(event: number) {
    this.uploadedFinancialIntegrity.emit(event);
  }
}
