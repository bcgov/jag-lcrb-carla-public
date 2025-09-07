import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { Application } from '@models/application.model';

/**
 * The personal history summary forms section of a permanent change application.
 *
 * @export
 * @class PermanentChangePersonalHistorySummaryFormsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-personal-history-summary-forms',
  templateUrl: './permanent-change-personal-history-summary-forms.component.html',
  styleUrls: ['./permanent-change-personal-history-summary-forms.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangePersonalHistorySummaryFormsComponent implements OnInit {
  @Input() application: Application;
  @Input() disabled: boolean = false;

  @Output() uploadedFinancialIntegrity = new EventEmitter<number>();

  faIdCard = faIdCard;

  isAmalgamated = false;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  onUploadedFinancialIntegrity(event: number) {
    this.uploadedFinancialIntegrity.emit(event);
  }
}
