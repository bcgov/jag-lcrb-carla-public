import { Component, OnInit, Input } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { PaymentDataService } from "@services/payment-data.service";
import { Subscription } from "rxjs";
import { ApplicationDataService } from "../../services/application-data.service";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Application } from "@models/application.model";
import { faAngleDoubleLeft, faPrint } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-payment-confirmation",
  templateUrl: "./payment-confirmation.component.html",
  styleUrls: ["./payment-confirmation.component.scss"]
})
/** payment-confirmation component*/
export class PaymentConfirmationComponent extends FormBase implements OnInit {
  faPrint = faPrint;
  faAngleDoubleLeft = faAngleDoubleLeft;
  busy: Subscription;
  htmlContent = {} as ApplicationHTMLContent;
  application: Application;
  transactionId: string;
  applicationId: string;
  authCode: string;
  avsMessage: string;
  avsAddrMatch: string;
  messageId: string;
  messageText: string;
  paymentMethod: string;
  cardType: string;
  trnAmount: string;
  trnApproved: string;
  trnDate: string;
  trnId: string;
  trnOrderNumber: string;
  invoice: string;
  isApproved = false;
  applicationType: string;
  isLiquor: boolean;
  retryCount = 0;

  paymentTransactionTitle: string;
  paymentTransactionMessage: string;
  loaded = false;
  @Input()
  inputApplicationId: string;

  static parseVerifyResponse(res) {
    const verifyPayResponse = res as any;
    let cardType: string = "";
    switch (verifyPayResponse.cardType) {
    case "VI":
      cardType = "Visa";
      break;
    case "PV":
      cardType = "Visa Debit";
      break;
    case "MC":
      cardType = "MasterCard";
      break;
    case "AM":
      cardType = "American Express";
      break;
    case "MD":
      cardType = "Debit MasterCard";
      break;
    default:
      cardType = verifyPayResponse.cardType;
    }
    const authCode = verifyPayResponse.authCode;
    const avsMessage = verifyPayResponse.avsMessage;
    const avsAddrMatch = verifyPayResponse.avsAddrMatch;
    const messageId = verifyPayResponse.messageId;
    const messageText = verifyPayResponse.messageText;
    const paymentMethod = verifyPayResponse.paymentMethod;
    const trnAmount = verifyPayResponse.trnAmount;
    const trnApproved = verifyPayResponse.trnApproved;
    const trnDate = verifyPayResponse.trnDate;
    const trnId = verifyPayResponse.trnId;
    const trnOrderNumber = verifyPayResponse.trnOrderNumber;
    const invoice = verifyPayResponse.invoice;
    let isApproved = false;
    let paymentTransactionTitle = "";
    let paymentTransactionMessage = "";

    if (trnApproved === "1") {
      isApproved = true;
    } else {
      isApproved = false;
      if (messageId === "559") {
        paymentTransactionTitle = "Cancelled";
        paymentTransactionMessage =
          "Your payment transaction was cancelled. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
      } else if (messageId === "7") {
        paymentTransactionTitle = "Declined";
        paymentTransactionMessage =
          "Your payment transaction was declined. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
      } else {
        paymentTransactionTitle = "Declined";
        paymentTransactionMessage =
          "Your payment transaction was declined. Please contact your bank for more information. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
      }
    }

    return {
      cardType,
      authCode,
      avsMessage,
      avsAddrMatch,
      messageId,
      messageText,
      paymentMethod,
      trnAmount,
      trnApproved,
      trnDate,
      trnId,
      trnOrderNumber,
      invoice,
      isApproved,
      paymentTransactionTitle,
      paymentTransactionMessage,
    };
  }

  /** payment-confirmation ctor */
  constructor(private router: Router,
    private route: ActivatedRoute,
    private paymentDataService: PaymentDataService,
    private applicationDataService: ApplicationDataService,
    public snackBar: MatSnackBar,
  ) {
    super();
    this.route.queryParams.subscribe(params => {
      this.transactionId = params["trnId"];
      this.applicationId = params["SessionKey"];
    });
  }

  ngOnInit() {
    if (!this.applicationId) {
      this.applicationId = this.inputApplicationId;
    }

    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        this.applicationType = data.applicationType.name;
        this.isLiquor = data.applicationType.category == "Liquor";
        this.application = data;
        this.addDynamicContent();
      });
  }

  ngAfterViewInit() {
    this.verify_payment();
    //this.getApplicationType();
  }


  /**
   * Payment verification
   * */
  verify_payment() {
    this.retryCount++;

    this.busy = this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
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
            this.paymentTransactionMessage =
              "Your payment transaction was cancelled. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
          } else if (this.messageId === "7") {
            this.paymentTransactionTitle = "Declined";
            this.paymentTransactionMessage =
              "Your payment transaction was declined. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
          } else {
            this.paymentTransactionTitle = "Declined";
            this.paymentTransactionMessage =
              "Your payment transaction was declined. Please contact your bank for more information. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>";
          }
        }

        this.loaded = true;
      },
      err => {
        if (err === "503" || err === "502" || err === "500") {
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
        this.loaded = true;
      }
    );
  }


  /**
   * Return to dashboard
   * */
  return_to_application() {
    if (this.trnApproved === "1") {
      this.router.navigate(["./dashboard"]);
    } else {
      this.router.navigate([`./application/${this.applicationId}`]);
    }
  }

  /**
   * Print current page
   * */
  printPage() {
    window.print();
  }
}
