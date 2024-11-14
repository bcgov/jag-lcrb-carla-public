import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { AppState } from "@app/app-state/models/app-state";
import { Contact } from "@models/contact.model";
import { SepApplication } from "@models/sep-application.model";
import { SepSchedule } from "@models/sep-schedule.model";
import { Store } from "@ngrx/store";
import { ContactDataService } from "@services/contact-data.service";
import { IndexedDBService } from "@services/indexed-db.service";
import { PaymentDataService } from "@services/payment-data.service";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { Subscription } from "rxjs";
import { map, mergeMap } from "rxjs/operators";
import { differenceInBusinessDays, isBefore } from "date-fns";
import {
  faAward,
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
import { MatDialog } from "@angular/material/dialog";
import { FinalConfirmationComponent } from "../final-confirmation/final-confirmation.component";
import { CancelSepApplicationDialogComponent } from "../cancel-sep-application-dialog/cancel-sep-application-dialog.component";

@Component({
  selector: "app-summary",
  templateUrl: "./summary.component.html",
  styleUrls: ["./summary.component.scss"]
})
export class SummaryComponent implements OnInit {
  @Input() account: any; // TODO: change to Account and fix prod error
  @Output() saveComplete = new EventEmitter<boolean>();
  busy: Subscription;
  mode: "readonlySummary" | "pendingReview" | "payNow" = "readonlySummary";
  _appID: number;
  SummaryComponent = SummaryComponent;
  application: SepApplication;
  faDownLoad = faDownload;
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
  /**
   * Controls whether or not the form show the submit button.
   * The value true by default
   */
  contact: Contact;
  transactionId: any;
  appId: any;
  retryCount: any;
  cardType: string;
  authCode: any;
  avsMessage: any;
  avsAddrMatch: any;
  messageId: any;
  messageText: any;
  paymentMethod: any;
  trnAmount: any;
  trnApproved: any;
  trnDate: any;
  trnId: any;
  trnOrderNumber: any;
  invoice: any;
  isApproved: boolean;
  paymentTransactionTitle: string;
  paymentTransactionMessage: string;
  loaded: boolean;
  savingToAPI: boolean;
  isRefundPolicyChecked: boolean = false;
  showRefundPolicyError:boolean = false;
  isDeclarationChecked: boolean = false;
  showDeclarationError: boolean = false;

  @Input() set localId(value: number) {
    this._appID = value;
    // get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        if (app && app.id) {
          this.setApplication(app.id);
        }
      });
  }

  get localId() {
    return this._appID;
  }

  constructor(
    private db: IndexedDBService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    private sepDataService: SpecialEventsDataService,
    private contactDataService: ContactDataService,
    private router: Router) {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        this.contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
          });
      });

    this.route.queryParams.subscribe(params => {
      this.transactionId = params["trnId"];
      this.appId = params["SessionKey"];
    });
    this.route.params.subscribe((params: Params) => {
      this.setApplication(params.apiId);
    });
  }

  setApplication(id: string) {
    if (id) {
      this.sepDataService.getSpecialEventForApplicant(id)
        .subscribe(async app => {
          this.application = app;
          this.formatEventDatesForDisplay();
        });
    }
  }

  ngOnInit(): void {
    if (this.transactionId) {
      this.verify_payment();

    }
    window.scrollTo(0, 0);
  }

  /**
 * Payment verification
 * */
  verify_payment() {
    this.retryCount++;
    this.paymentDataService.verifyPaymentURI("specialEventInvoice", this.appId).subscribe(
      res => {
        const verifyPayResponse = res as any;
        // console.log(verifyPayResponse);
        switch (verifyPayResponse.cardType) {
          case "VI":
            this.cardType = "Visa";
            break;
          case "PV":
            this.cardType = "Visa Debit";
            break;
          case "MC":
            this.cardType = "MasterCard";
            break;
          case "AM":
            this.cardType = "American Express";
            break;
          case "MD":
            this.cardType = "Debit MasterCard";
            break;
          default:
            this.cardType = verifyPayResponse.cardType;
        }
        this.authCode = verifyPayResponse.authCode;
        this.avsMessage = verifyPayResponse.avsMessage;
        this.avsAddrMatch = verifyPayResponse.avsAddrMatch;
        this.messageId = verifyPayResponse.messageId;
        this.messageText = verifyPayResponse.messageText;
        this.paymentMethod = verifyPayResponse.paymentMethod;
        this.trnAmount = verifyPayResponse.trnAmount;
        this.trnApproved = verifyPayResponse.trnApproved;
        this.trnDate = verifyPayResponse.trnDate;
        this.trnId = verifyPayResponse.trnId;
        this.trnOrderNumber = verifyPayResponse.trnOrderNumber;
        this.invoice = verifyPayResponse.invoice;

        if (this.trnApproved === "1") {
          this.isApproved = true;
        } else {
          this.isApproved = false;
          if (this.messageId === "559") {
            this.paymentTransactionTitle = "Cancelled";
            this.paymentTransactionMessage = `Your payment transaction was cancelled.
            Please note, you must pay your fees to receive your permit.`;
          } else if (this.messageId === "7") {
            this.paymentTransactionTitle = "Declined";
            this.paymentTransactionMessage = `Your payment transaction was declined.
            Please note, you must pay your fees to receive your permit.`;
          } else {
            this.paymentTransactionTitle = "Declined";
            this.paymentTransactionMessage =
              `Your payment transaction was declined. Please contact your bank for more information.
              Please note, you must pay your fees to receive your permit.`;
          }
        }

        this.loaded = true;
      },
      err => {
        if (err === "503") {
          if (this.retryCount < 30) {
            this.snackBar.open(`Attempt ${this.retryCount} at payment verification, please wait...`,
              "Verifying Payment",
              { duration: 3500, panelClass: ["red - snackbar"] });
            this.verify_payment();
          }
        } else {
          this.snackBar.open("An unexpected error occured, please contact the branch to check if payment was processed",
            "Verifying Payment",
            { duration: 3500, panelClass: ["red - snackbar"] });
          console.log("Unexpected Error occured:");
          console.log(err);
        }

      }
    );
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
  /*
    getStatusIcon(): IconDefinition {
      switch (this.application?.eventStatus) {
        case ("Pending Review"):
          return faStopwatch;
        case ("Approved"):
        case ("Reviewed"):
          return faCheck;
        case ("Issued"):
          return faAward;
        case ("Denied"):
        case ("Cancelled"):
          return faBan;
        default:
          return faStopwatch;
      }
    }
  */
  isReviewed(): boolean {
    return ["Approved", "Issued"].indexOf(this.application?.eventStatus) >= 0;
  }

  isDenied(): boolean {
    return false;
  }

  isSubmitted(): boolean {
    // if the Dynamics workflow fails, the application will be stuck in a submitted state. We should indicate that we're working on it.
    return ["Submitted"].indexOf(this.application?.eventStatus) >= 0;

  }

  isReadOnly(): boolean {
    return ["Pending Review", "Approved", "Issued", "Denied", "Cancelled"].indexOf(this.application?.eventStatus) >= 0;
  }

  isEventPast(): boolean {

    let diff = differenceInBusinessDays(new Date(this.application?.eventStartDate), new Date());
    return diff < 0;

  }


  getStatus(): string {
    // when an application happens in the past, sometimes we need to change the status in the front end.
    switch (this.application?.eventStatus) {
      case ("Pending Review"):
        if (this.isEventPast()) {
          return "Review Expired"
        } else {
          if (this.trnApproved === "1") {
            return "Issued";
          } else {
            return this.application?.eventStatus;
          }
        }
      case ("Approved"):
        if (this.isEventPast()) {
          return "Approval Expired"
        } else {
          if (this.trnApproved === "1") { // sometimes we return to the page after payment before the status can be updated on the backend, if we collected payment we're issued
            return "Issued";
          } else {
            return "Payment Required";
          }
        }
      default:
        return this.application?.eventStatus;
    }
  }

  getProfit(): number {
    return Math.max(this.application?.totalRevenue - this.application?.totalPurchaseCost, 0);
  }

  canWithdraw(): boolean {
    switch (this.getStatus()) {
      case ("Pending Review"):
      case ("Approved"):
      case ("Payment Required"):
        return true;
      default:
        return false;
    }
  }

  getStatusIcon(): IconDefinition {
    switch (this.getStatus()) {
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

  public static getPermitCategoryLabel(value: string): string {
    let res = "";
    switch (value) {
      case "Members":
        res = "Private – An organization's members or staff, invited guests and ticket holders";
        break;
      case "Family":
        res = "Private – Family and invited friends only";
        break;

      case "Hobbyist":
        res = "Private – Hobbyist competition";
        break;
      case "Anyone":
        res = "Public – Open to the general public or anyone who wishes to participate or buy a ticket";
        break;
    }
    return res;
  }



  async cancelApplication(): Promise<void> {

    // open dialog, get reference and process returned data from dialog
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
    dialogRef.afterClosed()
      .subscribe(async ([cancelApplication, reason]) => {
        if (cancelApplication) {
          if (this.application && this.application.id) {
            this.savingToAPI = true;
            const result = await this.sepDataService.updateSepApplication({ id: this.application.id, localId: this.localId, cancelReason: reason, eventStatus: "Cancelled" } as SepApplication, this.application.id)
              .toPromise();

            if (result.localId) {
              await this.db.applications.update(result.localId, result);
              this.localId = this.localId; // trigger data refresh
            }
            this.savingToAPI = false;
            this.router.navigate(["/sep/dashboard"]);
          }
        }
      });



  }

  async submitApplication(): Promise<void> {
    // check declaration
    if(!this.isDeclarationChecked){
      this.showDeclarationError = true;
      return;
    }

    this.showDeclarationError = false;
    this.savingToAPI = true;
    const appData = await this.db.getSepApplication(this.localId);

    if (appData.id) { // do an update ( the record exists in dynamics)
      const submitResult = await this.sepDataService.submitSepApplication(appData.id)
        .toPromise();

      const result = await this.sepDataService.getSpecialEventForApplicant(appData.id)
        .toPromise();

      if (result.eventStatus === "Approved") {
        this.mode = "payNow";
      } else if (result.eventStatus === "Pending Review") {
        this.mode = "pendingReview";
      } else {
        this.snackBar.open("The application is submitted.", "Success", { duration: 2500, panelClass: ["green-snackbar"] });
        this.router.navigateByUrl("/sep/my-applications");
      }
      if (this.localId) {
        await this.db.applications.update(this.localId, result);
        this.localId = this.localId; // trigger data refresh
      }
      //} else {
      //  const result = await this.sepDataService.createSepApplication({})

    }
    this.savingToAPI = false;
  }

  // present a confirmation dialog prior to the payment being processed.
  payNow() {
    this.validateRefundPolicyCheckbox();

    if (!this.isRefundPolicyChecked) {
      return;
    }

    // set dialogConfig settings
    const dialogConfig = {
      autoFocus: true,
      width: "500px",
      data: { id: this.application.id }
    };

    // open dialog, get reference and process returned data from dialog
    this.dialog.open(FinalConfirmationComponent, dialogConfig);
  }

  /**
   * Print current page
   * */
  printPage() {
    window.print();
  }

  validateRefundPolicyCheckbox() {
    this.showRefundPolicyError = !this.isRefundPolicyChecked;
  }

}


