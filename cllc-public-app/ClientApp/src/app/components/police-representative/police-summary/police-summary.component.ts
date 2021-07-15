import { ChangeDetectorRef, Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';
import { Account } from '@models/account.model';
import { AppState } from '@app/app-state/models/app-state';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { SepApplication } from '@models/sep-application.model';
import { AutoCompleteItem, SpecialEventsDataService } from '@services/special-events-data.service';
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
  faHandshake,
  faWineBottle,
  faFlag,
  faPencilAlt,
  faQuestionCircle,
  faShoppingCart,
  faStopwatch,
  faTrashAlt,
  IconDefinition,
  faBullhorn
} from "@fortawesome/free-solid-svg-icons";
import {
  faBan
} from "@fortawesome/free-solid-svg-icons";
import { FormBuilder, FormGroup } from '@angular/forms';
import { filter, tap, switchMap } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { SepSchedule } from '@models/sep-schedule.model';

@Component({
  selector: 'app-police-summary',
  templateUrl: './police-summary.component.html',
  styleUrls: ['./police-summary.component.scss']
})
export class PoliceSummaryComponent extends FormBase implements OnInit {
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
  faHandshake = faHandshake;
  faWineBottle = faWineBottle;
  faBolt = faBolt;
  faCheck = faCheck;
  faBan = faBan;
  busy: Subscription;
  specialEventId: string;
  contact: Contact;
  sepApplication: SepApplication;
  form: FormGroup;
  sepCityRequestInProgress: boolean;
  previewCities: AutoCompleteItem[] = [];
  autocompleteCities: AutoCompleteItem[] = [];
  validationMessages: string[];

  constructor(private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private specialEventsDataService: SpecialEventsDataService,
    private store: Store<AppState>,
    contactDataService: ContactDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    ) {

      super();
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

      specialEventsDataService.getSepCityAutocompleteData(null, true)
      .subscribe(results => {
        this.previewCities = results;
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
    this.form = this.fb.group({
      sepCity: ['']
    });
    this.busy =
    this.specialEventsDataService.getSpecialEventPolice(this.specialEventId)
      .subscribe(application => {
        this.sepApplication = application;
        this.formatEventDatesForDisplay();
        if (this.form && application) {
          this.form.patchValue(this.sepApplication);
        }
      });

      this.form.get('sepCity').valueChanges
      .pipe(filter(value => value && value.length >= 3),
        tap(_ => {
          this.autocompleteCities = [];
          this.sepCityRequestInProgress = true;
        }),
        switchMap(value => this.specialEventsDataService.getSepCityAutocompleteData(value, false))
      )
      .subscribe(data => {
        this.autocompleteCities = data;
        this.sepCityRequestInProgress = false;

        this.cd.detectChanges();
        if (data && data.length === 0) {
          this.snackBar.open('No match found', '', { duration: 2500, panelClass: ['green-snackbar'] });
        }
      });
  }


  formatEventDatesForDisplay() {
    if (this?.sepApplication?.eventLocations?.length > 0) {
      this.sepApplication.eventLocations.forEach(loc => {
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
    return this.sepApplication?.eventStatus == 'Approved' || this.sepApplication?.eventStatus == 'Issued';
  }

  isDenied(): boolean {
    return this.sepApplication?.eventStatus == 'Denied' || this.sepApplication?.eventStatus == 'Cancelled';
  }

  isCurrentUser(): boolean {
    return this.contact?.name == this.sepApplication.policeDecisionBy?.name;
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
    switch(this.sepApplication?.privateOrPublic){
      case ("Members"):
        return faHandshake;
      case ("Hobbyist"):
        return faWineBottle;
      case ("Anyone"):
        return faBullhorn;
      case ("Family"):
      default:
        return faBirthdayCake;

    }
  }

  getStatusIcon(): IconDefinition {
    switch(this.sepApplication?.eventStatus){
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
    for (var location of this.sepApplication.eventLocations) {
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


  getFormValue(): SepApplication {
    let formData = {
      ...this.sepApplication,
      ...this.form.value
    };

    const data = {
      ...formData
    } as SepApplication;
    return data;
  }

  setFormValue(app: SepApplication) {
    if (app) {
      this.form.patchValue(app);
    }
  }

  autocompleteDisplay(item: AutoCompleteItem) {
    return item?.name;
  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.form.updateValueAndValidity();
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid;
  }

  get cities(): AutoCompleteItem[] {
    return [...this.autocompleteCities, ...this.previewCities];
  }

  // update the municipality with the value chosen.
  updateMunicipality(): void {
    this.busy = this.specialEventsDataService.policeSetMunicipality(this.specialEventId, this.form.get('sepCity')?.value?.id)
    .subscribe(() => {
      this.snackBar.open("Set City.",
      "Success",
      { duration: 3500, panelClass: ["green-snackbar"] });
    },
      () => {
        this.snackBar.open('Error setting city', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.error('Error setting city');
      });
  }
}
