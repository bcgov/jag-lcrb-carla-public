import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Params } from "@angular/router";
import { AppState } from "@app/app-state/models/app-state";
import { Contact } from "@models/contact.model";
import { SepApplication } from "@models/sep-application.model";
import { SepSchedule } from "@models/sep-schedule.model";
import { Store } from "@ngrx/store";
import { ContactDataService } from "@services/contact-data.service";
import { IndexedDBService } from "@services/indexed-db.service";
import { PaymentDataService } from "@services/payment-data.service";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { map, mergeMap } from "rxjs/operators";

@Component({
  selector: "app-summary",
  templateUrl: "./summary.component.html",
  styleUrls: ["./summary.component.scss"]
})
export class SummaryComponent implements OnInit {
  @Input() account: any; // TODO: change to Account and fix prod error
  @Output() saveComplete = new EventEmitter<boolean>();
  mode: "readonlySummary" | "pendingReview" | "payNow" = "readonlySummary";
  _appID: number;
  application: SepApplication;
  /**
   * Controls whether or not the form show the submit button.
   * The value true by default
   */
  @Input() showSubmitButton = true;
  contact: Contact;

  @Input() set localId(value: number) {
    this._appID = value;
    // get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.application = app;
        this.formatEventDatesForDisplay();
      });
  }

  get localId() {
    return this._appID;
  }

  constructor(private db: IndexedDBService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    private sepDataService: SpecialEventsDataService,
    private contactDataService: ContactDataService) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
          });
      });
    route.params.subscribe((params: Params) => {
      const id = params.apiId;
      if (id) {
        sepDataService.getSpecialEventForApplicant(id)
          .subscribe(app => {
            this.showSubmitButton = false;
            this.application = app;
            this.formatEventDatesForDisplay();
          });
      }
    });
  }

  ngOnInit(): void {
  }

  formatEventDatesForDisplay() {
    if (this?.application?.eventLocations?.length > 0) {
      this.application.eventLocations.forEach(loc => {
        if (loc.eventDates?.length > 0) {
          const formatterdDates = [];
          loc.eventDates.forEach(ed => {
            ed = Object.assign(new SepSchedule(null), ed);
            formatterdDates.push({ ed, ...ed.toEventFormValue() });
          });
          loc.eventDates = formatterdDates;
        }
      });
    }
  }

  async submitApplication(): Promise<void> {
    const appData = await this.db.getSepApplication(this.localId);
    if (appData.id) { // do an update ( the record exists in dynamics)
      const result = await this.sepDataService.updateSepApplication({ ...appData, eventStatus: "Submitted" } as SepApplication, appData.id)
        .toPromise();
      if (result.eventStatus === "Approved") {
        this.mode = "payNow";
      } else if (result.eventStatus === "Pending Review") {
        this.mode = "pendingReview";
      }
      if (result.localId) {
        await this.db.applications.update(result.localId, result);
        this.localId = this.localId; // trigger data refresh
      }
    }
  }

  payNow() {
    // and payment is required due to an invoice being generated
    if (this?.application?.id) {
      // proceed to payment
      this.submitPayment()
        .subscribe(res => {
        },
          error => {
            this.snackBar.open("Error submitting payment", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
          }
        );
    }
  }

  /**
 * Redirect to payment processing page (Express Pay / Bambora service)
 * */
  private submitPayment() {
    return this.paymentDataService.getPaymentURI("specialEventInvoice", this.application.id)
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


