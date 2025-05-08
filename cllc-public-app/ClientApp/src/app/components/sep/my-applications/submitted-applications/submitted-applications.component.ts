import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { Router } from "@angular/router";
import { PoliceTableElement } from "@components/police-representative/police-table-element";
import { CancelSepApplicationDialogComponent } from "@components/sep/sep-application/cancel-sep-application-dialog/cancel-sep-application-dialog.component";
import { SEP_APPLICATION_STEPS } from "@components/sep/sep-application/sep-application.component";
import {
  faAward,
  faBan,
  faBirthdayCake,
  faBolt,
  faBusinessTime,
  faCalendarAlt,
  faCertificate,
  faCheck,
  faCopy,
  faDownload,
  faExchangeAlt,
  faExclamationTriangle,
  faFlag,
  faPencilAlt,
  faQuestionCircle,
  faShoppingCart,
  faStopwatch,
  faTrashAlt,
  IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import { SepApplicationSummary } from "@models/sep-application-summary.model";
import { SepApplication } from "@models/sep-application.model";
import { IndexedDBService } from "@services/indexed-db.service";
import { PaymentDataService } from "@services/payment-data.service";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { isBefore } from "date-fns";
import { map } from "rxjs/operators";

@Component({
  selector: "app-submitted-applications",
  templateUrl: "./submitted-applications.component.html",
  styleUrls: ["./submitted-applications.component.scss"],
})
export class SubmittedApplicationsComponent implements OnInit {
  faDownload = faDownload;
  faCopy = faCopy;
  faExclamationTriangle = faExclamationTriangle;
  faFlag = faFlag;
  faQuestionCircle = faQuestionCircle;
  faPencilAlt = faPencilAlt;
  faStopwatch = faStopwatch;
  faCertificate = faCertificate;
  faShoppingCart = faShoppingCart;
  faTrashAlt = faTrashAlt;
  faCalendarAlt = faCalendarAlt;
  faBusinessTime = faBusinessTime;
  faExchangeAlt = faExchangeAlt;
  faBirthdayCake = faBirthdayCake;
  faBolt = faBolt;
  faCheck = faCheck;
  faBan = faBan;

  @Input()
  set dataSourceOverride(value: MatTableDataSource<PoliceTableElement>) {
    this.dataSource = value;
  }
  get dataSourceOverride() {
    return this.dataSource;
  }

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  dataSource = new MatTableDataSource<PoliceTableElement>();

  // angular material table columns to display
  columnsToDisplay = [
    "eventStatusLabel",
    "eventName",
    "eventStartDate",
    "dateSubmitted",
    "actions",
  ];

  constructor(
    private sepDataService: SpecialEventsDataService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private db: IndexedDBService,
    private router: Router,
    private paymentDataService: PaymentDataService
  ) {}

  ngOnInit(): void {
    this.sepDataService
      .getSubmittedApplications()
      .subscribe((data) => (this.dataSource.data = data));
    if (this.dataSource) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }

  /**
   * Navigate to the application page.
   *
   * @param {SepApplicationSummary} app
   * @memberof SubmittedApplicationsComponent
   */
  openApplication(app: SepApplicationSummary) {
    if (app.eventStatus === "Draft") {
      this.router.navigateByUrl(
        `sep/application/${app.localId}/${this.getLastStep(
          app.lastStepCompleted
        )}`
      );
    } else {
      this.router.navigateByUrl(
        `sep/application-summary/${app.specialEventId}`
      );
    }
  }

  /**
   * Get the last step completed for the application.
   *
   * @param {string} stepCompleted
   * @return {*}  {string}
   * @memberof SubmittedApplicationsComponent
   */
  getLastStep(stepCompleted: string): string {
    const lastIndex = SEP_APPLICATION_STEPS.indexOf(stepCompleted);
    // return the next step to be completed
    return SEP_APPLICATION_STEPS[lastIndex + 1];
  }

  /**
   * Returns true if the event start date is before the current date.
   *
   * @param {string} eventStartDate
   * @return {*}  {boolean}
   * @memberof SubmittedApplicationsComponent
   */
  isEventPast(eventStartDate: string): boolean {
    return isBefore(new Date(eventStartDate), new Date());
  }

  /**
   * Get the human readable status of the application.
   *
   * @param {SepApplication} app
   * @return {*}  {string}
   * @memberof SubmittedApplicationsComponent
   */
  getStatus(app: SepApplication): string {
    // when an application happens in the past, sometimes we need to change the status in the front end.
    const isPast = this.isEventPast(String(app.eventStartDate));

    if (app.eventStatus === "Pending Review") {
      return isPast ? "Review Expired" : app.eventStatus;
    }

    if (app.eventStatus === "Approved") {
      return isPast ? "Approval Expired" : "Payment Required";
    }

    return app.eventStatus;
  }

  /**
   * Get the icon for the status of the application.
   *
   * @param {string} status
   * @return {*}  {IconDefinition}
   * @memberof SubmittedApplicationsComponent
   */
  getStatusIcon(status: string): IconDefinition {
    switch (status) {
      case "Pending Review":
        return faStopwatch;
      case "Draft":
        return faPencilAlt;
      case "Approved":
      case "Payment Required":
      case "Reviewed":
        return faCheck;
      case "Issued":
        return faAward;
      case "Denied":
      case "Cancelled":
      default:
        return faBan;
    }
  }

  /**
   * Clone an application.
   *
   * @param {SepApplicationSummary} appSummary
   * @memberof SubmittedApplicationsComponent
   */
  async cloneApplication(appSummary: SepApplicationSummary) {
    // first get the full application.
    this.sepDataService
      .getSpecialEventForApplicant(appSummary.specialEventId)
      .subscribe(async (app) => {
        const clone = { ...app };
        // clear dynamics IDs
        clone.id = undefined;
        clone.localId = undefined;
        clone.eventStatus = "Draft";

        // ensure the police field are cleared
        clone.policeDecisionBy = undefined;
        clone.policeApproval = undefined;

        if (clone?.eventLocations?.length > 0) {
          clone.eventLocations.forEach((loc) => {
            loc.id = undefined;
            if (loc?.serviceAreas?.length > 0) {
              loc.serviceAreas.forEach((area) => {
                area.id = undefined;
              });
            }
            if (loc?.eventDates?.length > 0) {
              loc.eventDates.forEach((ed) => {
                ed.id = undefined;
              });
            }
          });
        }

        const localId = await this.db.saveSepApplication({
          ...clone,
          dateAgreedToTsAndCs: undefined,
          isAgreeTsAndCs: false,
          dateCreated: new Date(),
        } as SepApplication);
        this.router.navigateByUrl(`/sep/application/${localId}/applicant`);
      });
  }

  /**
   * Cancel (withdraw) an application.
   *
   * @param {(SepApplicationSummary | SepApplication)} appSummary
   * @return {*}  {Promise<void>}
   * @memberof SubmittedApplicationsComponent
   */
  async cancelApplication(
    appSummary: SepApplicationSummary | SepApplication
  ): Promise<void> {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "600px",
      height: "500px",
      data: {
        showStartApp: false,
      },
    };

    const dialogRef = this.dialog.open(
      CancelSepApplicationDialogComponent,
      dialogConfig
    );

    dialogRef.afterClosed().subscribe(async ([cancelApplication, reason]) => {
      if (cancelApplication !== true) {
        return;
      }

      const applicationId = this.isSepApplicationSummary(appSummary)
        ? appSummary.specialEventId
        : appSummary.id;

      if (applicationId) {
        // If this application was persisted (submitted or persisted draft), update the application with a cancelled
        // status/reason.
        await this.sepDataService
          .updateSepApplication(
            {
              id: applicationId,
              cancelReason: reason,
              eventStatus: "Cancelled",
            } as SepApplication,
            applicationId
          )
          .toPromise();
      }

      // `localId` can be 0 so explicitly check for undefined or null
      if (appSummary.localId !== undefined && appSummary.localId !== null) {
        // If this application was cached locally (submitted or persisted draft or local draft), remove it from local
        // storage
        await this.db.applications.delete(Number(appSummary.localId));
      }

      this.router.navigateByUrl(`/sep/my-applications`).then(() => {
        window.location.reload();
      });
    });
  }

  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   *
   * @private
   * @param {string} applicationId
   * @return {*}
   * @memberof SubmittedApplicationsComponent
   */
  private submitPayment(applicationId: string) {
    return this.paymentDataService
      .getPaymentURI("specialEventInvoice", applicationId)
      .pipe(
        map(
          (jsonUrl) => {
            window.location.href = jsonUrl["url"];
            return jsonUrl["url"];
          },
          (err: any) => {
            if (err === "Payment already made") {
              this.snackBar.open(
                "Application payment has already been made, please refresh the page.",
                "Fail",
                { duration: 3500, panelClass: ["red-snackbar"] }
              );
            }
          }
        )
      );
  }

  /**
   * Type guard to check if the application is a SepApplicationSummary.
   *
   * @param {(SepApplication | SepApplicationSummary)} app
   * @memberof SubmittedApplicationsComponent
   */
  isSepApplicationSummary = (
    app: SepApplication | SepApplicationSummary
  ): app is SepApplicationSummary => {
    return (app as SepApplicationSummary).specialEventId !== undefined;
  };
}
