import { Component, Input, OnInit } from '@angular/core';
import { Account } from '@models/account.model';
import { FileSystemItem } from '@models/file-system-item.model';
import { AccountDataService } from '@services/account-data.service';

/**
 * The disclaimer section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-disclaimer',
  templateUrl: './legal-entity-review-disclaimer.component.html',
  styleUrls: ['./legal-entity-review-disclaimer.component.scss']
})
export class LegalEntityReviewDisclaimerComponent implements OnInit {
  /**
   * The account associated with the legal entity review application.
   */
  @Input() account: Account;

  /**
   * The notices related to the legal entity review application.
   */
  notices: FileSystemItem[] = [];

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
        // TODO: We need to figure out which notice(s) are relevant to the legal entity review, and update this logic accordingly.
        if (file.documenttype === 'LegalEntityReview TEMP') {
          this.notices.push(file);
        }
      });
    });
  }
}
