import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { faDownload } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { FileSystemItem } from '@models/file-system-item.model';
import { AccountDataService } from '@services/account-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { parseFileName } from '@shared/file-utils';
import { forkJoin } from 'rxjs';

/**
 * Renders one or more legal entity review outcome letters.
 *
 * @export
 * @class LegalEntityReviewOutcomeLetterComponent
 * @implements {OnChanges}
 */
@Component({
  selector: 'app-legal-entity-review-outcome-letter',
  templateUrl: './legal-entity-review-outcome-letter.component.html',
  styleUrls: ['./legal-entity-review-outcome-letter.component.scss']
})
export class LegalEntityReviewOutcomeLetterComponent implements OnChanges {
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

  faDownload = faDownload;

  constructor(private applicationDataService: ApplicationDataService, private accountDataService: AccountDataService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.account && changes.application) {
      // Fetch notices when both required inputs are available
      this.getLegalEntityReviewNotices();
    }
  }

  /**
   * Gets the notices related to the legal entity review application.
   */
  getLegalEntityReviewNotices(): void {
    if (!this.account) {
      return;
    }

    if (!this.application?.applicationExtension) {
      return;
    }

    forkJoin({
      // TODO: tiedhouse - Only fetching the related LE Review Application to get the Job Number. Likely a more
      // efficient way to achieve this, other than loading the entire application.
      relatedLEReviewApplication: this.applicationDataService.getApplicationById(
        this.application.applicationExtension?.relatedLeOrPclApplicationId
      ),
      allFiles: this.accountDataService.getFilesAttachedToAccount(this.account.id, 'Notice')
    }).subscribe({
      next: ({ relatedLEReviewApplication, allFiles }) => {
        if (!relatedLEReviewApplication) {
          // Failed to find the related LE Review Application, unable to identify the relevant notices.
          return;
        }

        if (!allFiles?.length) {
          // Failed to fetch any notices, nothing to display.
          return;
        }

        // Matches on `Notice__Legal_Entity_Review_Outcome_Letter_<job_number>.pdf`
        const regex = new RegExp(
          `Notice__Legal_Entity_Review_Outcome_Letter_${relatedLEReviewApplication.jobNumber}.pdf`
        );

        allFiles.forEach((file) => {
          if (regex.test(file.name)) {
            this.notices.push(file);
          }
        });
      }
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

    console.log(formattedFileName);

    // Fallback to the original fileName if formatting fails
    return formattedFileName || fileName;
  }
}
