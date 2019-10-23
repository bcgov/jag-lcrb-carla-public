import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { UserDataService } from '@services/user-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { AccountDataService } from '@services/account-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { Account } from '@models/account.model';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { Observable, forkJoin, of } from 'rxjs';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-associate-page',
  templateUrl: './associate-page.component.html',
  styleUrls: ['./associate-page.component.scss']
})
export class AssociatePageComponent extends FormBase implements OnInit {
  @Output() saveComplete: EventEmitter<boolean> = new EventEmitter<boolean>();
  account: Account;
  legalEntityId: string;
  lockAssociates = false;
  applicationId: string;
  application: Application;
  busy: any;
  showValidationMessages: boolean;
  validationMessages: string[];


  constructor(private store: Store<AppState>, private fb: FormBuilder,
    private applicationDataService: ApplicationDataService,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      renewalCriminalOffenceCheck: ['', Validators.required],
      renewalUnreportedSaleOfBusiness: ['', Validators.required],
      renewalBusinessType: ['', Validators.required],
      renewalTiedhouse: ['', Validators.required],
      tiedhouseFederalInterest: ['', Validators.required],
      renewalOrgLeadership: ['', Validators.required],
      renewalkeypersonnel: ['', Validators.required],
      renewalShareholders: ['', Validators.required],
      renewalOutstandingFines: ['', Validators.required],
    });
    this.subscribeForData();
  }


  reconfigureFormFields() {
    if (this.account.isPrivateCorporation() || this.account.isPublicCorporation()) {
      this.form.get('renewalShareholders').setValidators([Validators.required]);
      this.form.get('renewalShareholders').reset();
    } else {
      this.form.get('renewalShareholders').clearValidators();
      this.form.get('renewalShareholders').reset();
    }
  }


  subscribeForData() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => {
        this.account = account;
        this.reconfigureFormFields();
        this.legalEntityId = this.account.legalEntity.id;
      });

    this.busy = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, '');
        }
        if (data.applicantType === 'IndigenousNation') {
          (<any>data).applyAsIndigenousNation = true;
        }
        this.application = data;

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          }, {});

        this.form.patchValue(noNulls);
      },
        () => {
          console.log('Error occured');
        }
      );
  }

  save(showProgress: boolean = false): Observable<boolean> {
    const saveData = this.form.value;

    return forkJoin(
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value })
    ).pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        if (showProgress === true) {
          this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        }
        return of(true);
      }));
  }

  isValid(): boolean {
    // mark controls as touched
    for (const c in this.form.controls) {
      if (typeof (this.form.get(c).markAsTouched) === 'function') {
        this.form.get(c).markAsTouched();
      }
    }
    this.showValidationMessages = false;
    this.validationMessages = [];

    if (!this.form.valid) {
      this.validationMessages.push('Some required fields have not been completed');
    }
    return this.form.valid;
  }

  goToNextPage() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
    } else {
      this.busy = this.save().subscribe(_ => { this.saveComplete.emit(true); });
    }
  }

}
