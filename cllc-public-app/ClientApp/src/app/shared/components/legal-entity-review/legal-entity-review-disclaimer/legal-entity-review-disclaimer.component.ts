import { Component, Input } from '@angular/core';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';

/**
 * The disclaimer section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewComponent
 */
@Component({
  selector: 'app-legal-entity-review-disclaimer',
  templateUrl: './legal-entity-review-disclaimer.component.html',
  styleUrls: ['./legal-entity-review-disclaimer.component.scss']
})
export class LegalEntityReviewDisclaimerComponent {
  /**
   * The account associated with the legal entity review application.
   */
  @Input() account: Account;
  /**
   * The legal entity review application.
   */
  @Input() application: Application;
}
