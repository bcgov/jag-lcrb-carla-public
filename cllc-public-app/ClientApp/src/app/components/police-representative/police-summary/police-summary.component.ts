import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';
import { Account } from '@models/account.model';
import { AppState } from '@app/app-state/models/app-state';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { SepApplication } from '@models/sep-application.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ContactDataService } from '@services/contact-data.service';
import { Contact } from '@models/contact.model';
import { AcceptDialogComponent } from '@components/police-representative/police-summary/accept-dialog/accept-dialog.component';
import { DenyDialogComponent } from './deny-dialog/deny-dialog.component';
import { CancelDialogComponent } from '@components/police-representative/police-summary/cancel-dialog/cancel-dialog.component';

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



@Component({
  selector: 'app-police-summary',
  templateUrl: './police-summary.component.html',
  styleUrls: ['./police-summary.component.scss']
})
export class PoliceSummaryComponent implements OnInit {
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
  busy: Subscription;
  specialEventId: string;
  contact: Contact;
  public application: SepApplication;

  constructor(private specialEventsDataService: SpecialEventsDataService,
    private store: Store<AppState>,
    contactDataService: ContactDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    ) {

      this.route.paramMap.subscribe(params => {
        this.specialEventId = params.get("specialEventId");
      });

      store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
          });
      });
  }
/*
  constructor(private fb: FormBuilder,
    store: Store<AppState>,
    contactDataService: ContactDataService,
    private db: IndexedDBService) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
          });
      });
  }
*/

  ngOnInit(): void {
    console.log ("INIT");
    console.log (this.specialEventId);
    this.busy =
    this.specialEventsDataService.getSpecialEvent(this.specialEventId)
      .subscribe(application => {
        this.application = application;
        console.log (application);
      });
  }

  approve(): void {

    // open dialog, get reference and process returned data from dialog
    const dialogConfig = {
        disableClose: true,
        autoFocus: true,
        width: '500px',
        height: '300px',
        data: {
          showStartApp: false
        }
      };

    const dialogRef = this.dialog.open(AcceptDialogComponent, dialogConfig);
    dialogRef.afterClosed()
      //.pipe(takeWhile(() => this.componentActive))
      .subscribe(acceptApplication => {
        if (acceptApplication) {
          // delete the application.
          this.busy = this.specialEventsDataService.policeApproveSepApplication(this.specialEventId)
            .subscribe(() => {
              this.snackBar.open("Reviewed application.",
              "Success",
              { duration: 3500, panelClass: ["green-snackbar"] })
              this.router.navigate(['/sep/police/my-jobs']);
            },
              () => {
                this.snackBar.open('Error reviewing the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
                console.error('Error reviewing the application');
              });
        }
      });

  }

  isReviewed(): boolean {
    return this.application?.eventStatus == 'Approved' || this.application?.eventStatus == 'Issued';
  }

  isDenied(): boolean {
    return this.application?.eventStatus == 'Denied' || this.application?.eventStatus == 'Cancelled';
  }

  isCurrentUser(): boolean {
    return this.contact?.name == this.application.policeDecisionBy?.name;
  }

  getSize(guests: number): string{

    if(guests < 101){
      return "Small";
    } else {
      if(guests < 500){
        return "Medium";
      } else {
          return "Large";
        }
      }
    }

  getTypeIcon(): IconDefinition{
    let icon = faBirthdayCake;
    return icon;
  }

  getStatusIcon(): IconDefinition {
    switch(this.application?.eventStatus){
      case ("PendingReview"):
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

  getNumberOfLocations(): number {
    let num = 0;
    for (var location of this.application.eventLocations) {
      num++;
    }
    return num;
  }

  deny(): void {

        // open dialog, get reference and process returned data from dialog
        const dialogConfig = {
          disableClose: true,
          autoFocus: true,
          width: '500px',
          height: '500px',
          data: {
            showStartApp: false
          }
        };

      const dialogRef = this.dialog.open(DenyDialogComponent, dialogConfig);
      dialogRef.afterClosed()
        //.pipe(takeWhile(() => this.componentActive))
        .subscribe(cancelApplication => {
          if (cancelApplication) {

            this.busy = this.specialEventsDataService.policeDenySepApplication(this.specialEventId)
              .subscribe(() => {
                this.snackBar.open("Denied application.",
                "Success",
                { duration: 3500, panelClass: ["green-snackbar"] })
                this.router.navigate(['/sep/police/my-jobs']);
              },
                () => {
                  this.snackBar.open('Error denying the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
                  console.error('Error denying the application');
                });
          }
        });

  }

  revoke(): void {

    // open dialog, get reference and process returned data from dialog
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '500px',
      height: '500px',
      data: {
        showStartApp: false
      }
    };

  const dialogRef = this.dialog.open(CancelDialogComponent, dialogConfig);
  dialogRef.afterClosed()
    //.pipe(takeWhile(() => this.componentActive))
    .subscribe(cancelApplication => {
      if (cancelApplication) {

        this.busy = this.specialEventsDataService.policeCancelSepApplication(this.specialEventId)
          .subscribe(() => {
            this.snackBar.open("Cancelled application.",
            "Success",
            { duration: 3500, panelClass: ["green-snackbar"] })
            this.router.navigate(['/sep/police/my-jobs']);
          },
            () => {
              this.snackBar.open('Error cancelling the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
              console.error('Error cancelling the application');
            });
      }
    });

}


}


