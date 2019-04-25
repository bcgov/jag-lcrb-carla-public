
import {filter,  auditTime } from 'rxjs/operators';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { ApplicationDataService } from '../../../services/application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription ,  Subject, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { UserDataService } from '../../../services/user-data.service';

import * as currentApplicationActions from '../../../app-state/actions/current-application.action';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import { Application } from '../../../models/application.model';

@Component({
  selector: 'app-store-information',
  templateUrl: './store-information.component.html',
  styleUrls: ['./store-information.component.scss']
})
export class StoreInformationComponent implements OnInit, OnDestroy {

  applicationId: string;
  storeInformationForm: FormGroup;
  busy: Subscription;
  subscriptions: Subscription[] = [];
  savedFormData: any = {};

  constructor(private applicationDataService: ApplicationDataService,
    private store: Store<AppState>,
    private fb: FormBuilder,
    private userDataService: UserDataService,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.createForm();

    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication).pipe(
      filter(state => !!state))
      .subscribe(currentApplication => {
        this.storeInformationForm.patchValue(currentApplication);
        if (currentApplication.isPaid) {
          this.storeInformationForm.disable();
        }
        this.savedFormData = this.storeInformationForm.value;
      });
    this.subscriptions.push(sub);

    // const sub2 = this.storeInformationForm.valueChanges
    //   .pipe(auditTime(10000))
    //   .filter(formData => (JSON.stringify(formData) !== JSON.stringify(this.savedFormData)))
    //   .subscribe(formData => {
    //     this.save();
    //   });
    // this.subscriptions.push(sub2);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  /**
   * Create the entry form
   * */
  createForm() {
    this.storeInformationForm = this.fb.group({
      id: [''],
      establishmentName: [''], // Validators.required
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.savedFormData) === JSON.stringify(this.storeInformationForm.value)) {
      return true;
    } else {
      return this.save(true);
    }
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const saveData = this.storeInformationForm.value;
    const subscription = this.applicationDataService.updateApplication(this.storeInformationForm.value).subscribe(
      res => {
        saveResult.next(true);
        this.savedFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true) {
          this.snackBar.open('Store Information has been saved', 'Success', { duration: 2500, panelClass: ['red-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Store Information', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured');
      });

    if (showProgress === true) {
      this.busy = subscription;
    }

    return saveResult;
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application ) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

  isFieldError(field: string) {
    const isError = !this.storeInformationForm.get(field).valid && this.storeInformationForm.get(field).touched;
    return isError;
  }

}
