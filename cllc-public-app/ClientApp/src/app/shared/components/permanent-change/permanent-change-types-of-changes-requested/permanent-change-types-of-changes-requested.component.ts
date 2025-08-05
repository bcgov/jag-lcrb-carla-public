import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { PCLFormControlDefinitionOption } from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-types';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
/**
 * The types of changes section of a permanent change application.
 *
 * @export
 * @class PermanentChangeTypesOfChangesRequestedComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-types-of-changes-requested',
  templateUrl: './permanent-change-types-of-changes-requested.component.html',
  styleUrls: ['./permanent-change-types-of-changes-requested.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeTypesOfChangesRequestedComponent implements OnInit {
  @Input() account: Account;
  @Input() changeList: PCLFormControlDefinitionOption[];

  faQuestionCircle = faQuestionCircle;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
