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

  async cloneApplication(app: SepApplication) {
    const clone = { ...app };
    // clear dynamics IDs
    clone.id = undefined;
    clone.localId = undefined;
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
        if (err._body === "Payment already made") {
          this.snackBar.open("Application payment has already been made.", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        }
      }));
  }

}
