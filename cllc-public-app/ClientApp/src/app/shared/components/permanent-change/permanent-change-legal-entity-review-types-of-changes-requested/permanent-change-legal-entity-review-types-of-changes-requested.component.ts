import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';

/**
 * The types of changes section of a permanent change legal entity review application.
 *
 * @export
 * @class PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-legal-entity-review-types-of-changes-requested',
  templateUrl: './permanent-change-legal-entity-review-types-of-changes-requested.component.html',
  styleUrls: ['./permanent-change-legal-entity-review-types-of-changes-requested.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent implements OnInit {
  @Input() account: Account;
  @Input() changeList: any[];

  faQuestionCircle = faQuestionCircle;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
