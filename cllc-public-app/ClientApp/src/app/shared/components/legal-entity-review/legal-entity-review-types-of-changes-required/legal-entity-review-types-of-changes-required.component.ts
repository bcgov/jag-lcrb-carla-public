import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { PermanentChangeTypesOfChangesOption } from '@app/constants/permanent-change-types-of-changes';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';

/**
 * The types of changes required section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewTypesOfChangesRequiredComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-types-of-changes-required',
  templateUrl: './legal-entity-review-types-of-changes-required.component.html',
  styleUrls: ['./legal-entity-review-types-of-changes-required.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewTypesOfChangesRequiredComponent implements OnInit {
  @Input() account: Account;
  @Input() changeList: PermanentChangeTypesOfChangesOption[];

  faQuestionCircle = faQuestionCircle;

  form: FormGroup;

  typesOfChangesRequired: PermanentChangeTypesOfChangesOption[] = [];

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;

    for (const changeListItem of this.changeList) {
      const formControlName = changeListItem.formControlName;

      if (this.form.get(formControlName)?.value) {
        this.typesOfChangesRequired.push(changeListItem);
      }
    }
  }
}
