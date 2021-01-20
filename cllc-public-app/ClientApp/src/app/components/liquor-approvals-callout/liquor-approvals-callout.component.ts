import { Component, OnInit, Input } from "@angular/core";
import { Account } from "@models/account.model";
import { ApplicationDataService } from "@services/application-data.service";
import { Application } from "@models/application.model";
import { ApplicationType, ApplicationTypeNames } from "@models/application-type.model";

@Component({
  selector: "app-liquor-approvals-callout",
  templateUrl: "./liquor-approvals-callout.component.html",
  styleUrls: ["./liquor-approvals-callout.component.scss"]
})
export class LiquorApprovalsCalloutComponent implements OnInit {

  @Input()
  account: Account;

  nonTerminatedApplicationExists: boolean;
  terminatedApplicationExists: boolean;
  applications: Application[];
  busy: any;

  constructor(private applicationDataService: ApplicationDataService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.busy = this.applicationDataService.getApplicationsByType(ApplicationTypeNames.LGINClaim)
      .subscribe(applications => {
        this.applications = applications;

        // check whether there is an application that is not terminated
        this.nonTerminatedApplicationExists =
          applications.filter(app => app.applicationStatus !== "Terminated")
          .length >
          0;

        // check whether there is an application that is terminated
        this.terminatedApplicationExists =
          applications.filter(app => app.applicationStatus === "Terminated")
          .length >
          0;
      });
  }

  requestApprovals() {
    const application = {
      applicantType: this.account.businessType,
      applicationType: { name: ApplicationTypeNames.LGINClaim } as ApplicationType,
      account: this.account,
    } as Application;
    this.applicationDataService.createApplication(application)
      .subscribe(result => {
        this.loadData();
      });
  }

}
