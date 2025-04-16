import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { PoliceTableElement } from "@components/police-representative/police-table-element";
import { SepApplicationSummary } from "@models/sep-application-summary.model";
import { SepApplication } from "@models/sep-application.model";
import { PaymentDataService } from "@services/payment-data.service";
import { IndexedDBService } from "@services/indexed-db.service";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { map } from "rxjs/operators";
import { isBefore } from "date-fns";
import {
  faAward,
  faCopy,
  faBirthdayCake,
  faBolt,
  faBusinessTime,
  faCalendarAlt,
  faCertificate,
  faCheck,
  faDownload,
  faExchangeAlt,
  faExclamationTriangle,
  faFlag,
  faPencilAlt,
  faQuestionCircle,
  faShoppingCart,
  faStopwatch,
  faTrashAlt,
  IconDefinition
} from "@fortawesome/free-solid-svg-icons";
import {
  faBan
} from "@fortawesome/free-solid-svg-icons";
import { SEP_APPLICATION_STEPS } from "@components/sep/sep-application/sep-application.component";
import { Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { CancelSepApplicationDialogComponent } from "@components/sep/sep-application/cancel-sep-application-dialog/cancel-sep-application-dialog.component";

@Component({
  selector: "app-submitted-applications",
  templateUrl: "./submitted-applications.component.html",
  styleUrls: ["./submitted-applications.component.scss"]
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
    "eventStatusLabel", "eventName", "eventStartDate", "dateSubmitted", "actions"
  ];

  constructor(private sepDataService: SpecialEventsDataService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private db: IndexedDBService,
    private router: Router,
    private paymentDataService: PaymentDataService) { }

  ngOnInit(): void {
    this.sepDataService.getSubmittedApplications()
      .subscribe(data => this.dataSource.data = data);
    if(this.dataSource){
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }


  openApplication(app: SepApplicationSummary) {
    if (app.eventStatus === "Draft") {
      this.router.navigateByUrl(`sep/application/${app.localId}/${this.getLastStep(app.lastStepCompleted)}`);
    } else {
      this.router.navigateByUrl(`sep/application-summary/${app.specialEventId}`);
    }
  }



  getLastStep(stepCompleted: string): string {
    const lastIndex = SEP_APPLICATION_STEPS.indexOf(stepCompleted);
    // return the next step to be completed
    return SEP_APPLICATION_STEPS[lastIndex + 1];
  }

  payNow(applicationId: string) {
    // and payment is required due to an invoice being generated
    if (applicationId) {
      // proceed to payment
      this.submitPayment(applicationId)
        .subscribe(res => {
        },
          error => {
            this.snackBar.open("Error submitting payment", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
          }
        );
    }
  }

  isEventPast(eventStartDate: string) {
    return isBefore(new Date(eventStartDate), new Date() );
  }

  getStatus(app: SepApplication): string {
    // when an application happens in the past, sometimes we need to change the status in the front end.
      switch(app.eventStatus) {
        case ("Pending Review"):
          if(this.isEventPast(String(app.eventStartDate))){
            return "Review Expired"
          } else {
            return app.eventStatus;
          }
        case ("Approved"):
          if(this.isEventPast(String(app.eventStartDate))){
            return "Approval Expired"
          } else {
            return "Payment Required";
          }
        default:
          return app.eventStatus;
      }
  }

  getStatusIcon(status: string): IconDefinition {
    switch (status) {
      case ("Pending Review"):
        return faStopwatch;
      case ("Draft"):
        return faPencilAlt;
      case ("Approved"):
      case ("Payment Required"):
      case ("Reviewed"):
        return faCheck;
      case ("Issued"):
        return faAward;
      case ("Denied"):
      case ("Cancelled"):
      default:
        return faBan;
    }
  }

  async cloneApplication(appSummary: SepApplicationSummary) {

    // first get the full application.
    this.sepDataService.getSpecialEventForApplicant(appSummary.specialEventId)
      .subscribe(async app => {
        const clone = { ...app };
        // clear dynamics IDs
        clone.id = undefined;
        clone.localId = undefined;
        clone.eventStatus= "Draft";

        // ensure the police field are cleared
        clone.policeDecisionBy = undefined;
        clone.policeApproval = undefined;

        if (clone?.eventLocations?.length > 0) {
          clone.eventLocations.forEach(loc => {
            loc.id = undefined;
            if (loc?.serviceAreas?.length > 0) {
              loc.serviceAreas.forEach(area => {
                area.id = undefined;
              });
            }
            if (loc?.eventDates?.length > 0) {
              loc.eventDates.forEach(ed => {
                ed.id = undefined;
              });
            }
          });
        }
    
        const localId = await this.db.saveSepApplication({
          ...clone,
          dateAgreedToTsAndCs: undefined,
          isAgreeTsAndCs: false,
          dateCreated: new Date()
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
  async cancelApplication(appSummary: SepApplicationSummary | SepApplication): Promise<void> {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "600px",
      height: "500px",
      data: {
        showStartApp: false
      }
    };

    const dialogRef = this.dialog.open(CancelSepApplicationDialogComponent, dialogConfig);

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

  // async getApplications() {
  //   let applications = await this.db.applications.toArray();
  //   applications = applications.filter(app => app.eventStatus === 'Draft');
  //   applications = applications.sort((a, b) => {
  //     const dateA = new Date(a.dateCreated).getTime();
  //     const dateB = new Date(b.dateCreated).getTime();
  //     return dateB - dateA;
  //   });
  //   this.applications = applications;
  // }

  /**
 * Redirect to payment processing page (Express Pay / Bambora service)
 * */

  private submitPayment(applicationId: string) {
    return this.paymentDataService.getPaymentURI("specialEventInvoice", applicationId)
      .pipe(map(jsonUrl => {
        window.location.href = jsonUrl["url"];
        return jsonUrl["url"];
      }, (err: any) => {
        if (err === "Payment already made") {
          this.snackBar.open("Application payment has already been made, please refresh the page.", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        }
      }));
  }

  isSepApplicationSummary = (
    app: SepApplication | SepApplicationSummary
  ): app is SepApplicationSummary => {
    return (app as SepApplicationSummary).specialEventId !== undefined;
  }
}
