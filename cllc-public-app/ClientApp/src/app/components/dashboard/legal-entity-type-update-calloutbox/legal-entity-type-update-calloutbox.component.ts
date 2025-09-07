import { Component, Input, OnInit } from '@angular/core';
import { Account } from '@models/account.model';
import { ApplicationDataService } from '@services/application-data.service';

@Component({
  selector: 'app-legal-entity-type-update-calloutbox',
  templateUrl: './legal-entity-type-update-calloutbox.component.html',
  styleUrls: ['./legal-entity-type-update-calloutbox.component.scss']
})
export class LegalEntityTypeUpdateCalloutboxComponent implements OnInit {
  @Input() account: Account;

  /**
   * Whether the data has been loaded.
   */
  hasLoadedData: boolean = false;

  /**
   * Whether the user is allowed to create a PCL application
   */
  canCreatePCLApplication: boolean = true;

  constructor(private applicationsService: ApplicationDataService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.applicationsService.getInProgressLegalEntityReviewApplications().subscribe((inProgressLeReviewApplications) => {
      this.canCreatePCLApplication = !inProgressLeReviewApplications || inProgressLeReviewApplications.length === 0;

      this.hasLoadedData = true;
    });
  }
}
