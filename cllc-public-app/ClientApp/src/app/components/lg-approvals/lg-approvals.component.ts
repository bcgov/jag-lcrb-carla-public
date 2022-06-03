import { Component, OnInit } from "@angular/core";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { Account } from "@models/account.model";
import { ApplicationDataService } from "@services/application-data.service";
import { Application } from "@models/application.model";
import { MatSnackBar } from "@angular/material/snack-bar";
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { startOfToday, add, differenceInDays } from "date-fns";

@Component({
  selector: "app-lg-approvals",
  templateUrl: "./lg-approvals.component.html",
  styleUrls: ["./lg-approvals.component.scss"]
})
export class LgApprovalsComponent implements OnInit {
  faPencilAlt = faPencilAlt;
  account: Account;
  applications: Application[];
  applicationsDecisionNotMade: Application[] = [];
  applicationsForZoning: Application[] = [];
  applicationsDecisionMadeButNoDocs: Application[] = [];
  busy: any;
  dataLoaded = false; // this is set to true when all page data is loaded

  constructor(private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private applicationDataService: ApplicationDataService) {
  }

  ngOnInit() {
    // get account
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => {
        this.account = account;
      });

    // get approval applications split into 3 parts
    // THIS is original one.
    //this.busy = this.applicationDataService.getLGApprovalApplications()
    //  .subscribe(applications => {
    //    this.applications = applications || [];
    //    this.applicationsDecisionNotMade =
    //      this.applications.filter(app => !app.lGDecisionSubmissionDate &&
    //        app.applicationType &&
    //        (app.applicationType.isShowLGINApproval ||
    //          (app.applicationStatus === "Pending for LG/FN/Police Feedback"
    //            && app?.applicationType?.isShowLGZoningConfirmation !== true
    //          )
    //        )
    //      );

    //    this.applicationsForZoning =
    //      this.applications.filter(app => !app.lGDecisionSubmissionDate &&
    //        app.applicationType &&
    //        app.applicationType.isShowLGZoningConfirmation);

    //    this.applicationsDecisionMadeButNoDocs =
    //      this.applications.filter(app => app.lGDecisionSubmissionDate && app.lGApprovalDecision === "Pending");
    //    this.dataLoaded = true;
    //  },
    //    error => {
    //      this.snackBar.open(`An error occured while getting approval applications`,
    //        "Fail",
    //        { duration: 3500, panelClass: ["red-snackbar"] });
    //    });
    this.dataLoaded = true;
  }

  get90dayCount(submissionDate: Date): number {
    const submission = add(new Date(submissionDate), { days: 90});
    const count =  differenceInDays(startOfToday(), submission);
    return count;
  }

  noApplications(): boolean {
    const res = this.applicationsDecisionNotMade.length === 0 &&
      this.applicationsDecisionMadeButNoDocs.length === 0 &&
      this.applicationsForZoning.length === 0;
    return res;
  }
}
