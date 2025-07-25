import { Component, Input, OnInit } from '@angular/core';
import { Account } from '@models/account.model';
import { FileSystemItem } from '@models/file-system-item.model';
import { AccountDataService } from '@services/account-data.service';

/**
 * Renders one or more legal entity review outcome letters.
 *
 * @export
 * @class LegalEntityReviewOutcomeLetterComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-outcome-letter',
  templateUrl: './legal-entity-review-outcome-letter.component.html',
  styleUrls: ['./legal-entity-review-outcome-letter.component.scss']
})
export class LegalEntityReviewOutcomeLetterComponent implements OnInit {
  /**
   * The account associated with the legal entity review application.
   */
  @Input() account: Account;

  /**
   * The notices related to the legal entity review application.
   */
  notices: FileSystemItem[] = [];

  readonly legalEntityReviewOutcomeLetterName = 'Notice__Legal_Entity_Review_Outcome_Letter.pdf';

  constructor(private accountDataService: AccountDataService) {}

  ngOnInit() {
    this.getLegalEntityReviewNotices();
  }

  /**
   * Gets the notices related to the legal entity review application.
   */
  getLegalEntityReviewNotices(): void {
    this.accountDataService.getFilesAttachedToAccount(this.account.id, 'Notice').subscribe((data) => {
      if (!data?.length) {
        return;
      }

      data.forEach((file) => {
        // This assumes that the name of the legal entity review outcome letter is consistent/static.
        if (file.name === this.legalEntityReviewOutcomeLetterName) {
          this.notices.push(file);
        }
      });
    });
  }
}
