import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';
import { PaymentDataService } from '@services/payment-data.service';
import { map, mergeMap } from "rxjs/operators";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { ActivatedRoute, Params } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { ContactDataService } from '@services/contact-data.service';
import { Contact } from '@models/contact.model';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-final-confirmation',
  templateUrl: './final-confirmation.component.html',
  styleUrls: ['./final-confirmation.component.scss']
})
export class FinalConfirmationComponent implements OnInit {
  @Input() account: any; // TODO: change to Account and fix prod error
  @Output() saveComplete = new EventEmitter<boolean>();
  mode: "readonlySummary" | "pendingReview" | "payNow" = "readonlySummary";
  _appID: number;
  application: SepApplication;
  contact: Contact;
  appId: any;
  
  constructor(
    private snackBar: MatSnackBar,
    private db: IndexedDBService,
    private route: ActivatedRoute,
    private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    private sepDataService: SpecialEventsDataService,
    private contactDataService: ContactDataService,
    @Inject(MAT_DIALOG_DATA) public data: any
    )
    
    { 

      this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        this.contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
          });
      });

      if (data) {
        this.setApplication(data.id);
      } 
    }

  ngOnInit(): void {
  }

  @Input() set localId(value: number) {
    this._appID = value;
    // get the last saved application
    this.db.getSepApplication(value)
      .then(app => {        
      });
  }

  get localId() {
    return this._appID;
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

  setApplication(id: string) {
    
    if (id) {
      this.sepDataService.getSpecialEventForApplicant(id)
        .subscribe(app => {
          this.application = app;
          //this.formatEventDatesForDisplay();
        });
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
