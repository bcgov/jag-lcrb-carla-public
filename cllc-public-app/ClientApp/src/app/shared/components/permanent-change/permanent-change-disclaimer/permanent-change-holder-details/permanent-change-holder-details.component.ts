import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The disclaimer section of a permanent change application.
 *
 * @export
 * @class PermanentChangeHolderDetailsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-holder-details',
  templateUrl: './permanent-change-holder-details.component.html',
  styleUrls: ['./permanent-change-holder-details.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeHolderDetailsComponent implements OnInit {
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
