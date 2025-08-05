import { Component, Input } from '@angular/core';
import { PCLFormControlDefinitionOption } from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-types';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';

/**
 * The types of changes required section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewTypesOfChangesRequiredComponent
 */
@Component({
  selector: 'app-legal-entity-review-types-of-changes-required',
  templateUrl: './legal-entity-review-types-of-changes-required.component.html',
  styleUrls: ['./legal-entity-review-types-of-changes-required.component.scss']
})
export class LegalEntityReviewTypesOfChangesRequiredComponent {
  /**
   * The account associated with the legal entity review application.
   *
   * @type {Account}
   */
  @Input() account: Account;
  /**
   * The list of changes required for the permanent change application.
   *
   * @type {PCLFormControlDefinitionOption[]}
   */
  @Input() changesRequired: PCLFormControlDefinitionOption[];

  faQuestionCircle = faQuestionCircle;
}
