import { Component, Input, OnInit } from '@angular/core';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { FileSystemItem } from '@models/file-system-item.model';
import { AccountDataService } from '@services/account-data.service';
import { parseFileName } from '@shared/file-utils';

/**
 * Renders one or more legal entity review notice letters.
 *
 * @export
 * @class LegalEntityReviewNoticeLetterComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-notice-letter',
  templateUrl: './legal-entity-review-notice-letter.component.html',
  styleUrls: ['./legal-entity-review-notice-letter.component.scss']
})
export class LegalEntityReviewNoticeLetterComponent implements OnInit {
  /**
   * The account associated with the legal entity review application.
   */
  @Input() account: Account;
  /**
   * The legal entity review application.
   */
  @Input() application: Application;

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
    this.accountDataService.getFilesAttachedToAccount(this.account.id, 'Notice').subscribe((allFiles) => {
      if (!allFiles?.length) {
        return;
      }

      // Matches on `Notice__Legal_Entity_Review_Notice_Letter_<job_number>.pdf`
      const regex = new RegExp(`Notice__Legal_Entity_Review_Notice_Letter_${this.application.jobNumber}.pdf`);

      allFiles.forEach((file) => {
        if (regex.test(file.name)) {
          this.notices.push(file);
        }
      });
    });
  }

  /**
   * Formats the raw file name by removing the type/category prefix and removing the job number.
   *
   * @param {string} fileName
   * @return {*}  {string}
   */
  getFormattedFileName(fileName: string): string {
    // Split the fileName into its type/category and name parts
    const parsedFileName = parseFileName(fileName);

    // Matches on `(<any_string>)(_<job_number>)(.<any_string>)`
    // Example: `(Legal_Entity_Review_Notice_Letter)(_123456)(.pdf)`
    const regex = /^(.*)(_\d+)(\..*)$/;

    // Remove the job number
    const formattedFileName = parsedFileName.fileName.replace(regex, '$1$3');

    // Fallback to the original fileName if formatting fails
    return formattedFileName || fileName;
  }
}
