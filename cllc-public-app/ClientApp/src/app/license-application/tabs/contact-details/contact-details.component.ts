import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { auditTime } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import * as currentApplicationActions from '../../../app-state/actions/current-application.action';
import { AdoxioApplication } from '../../../models/adoxio-application.model';

@Component({
  selector: 'app-contact-details',
  templateUrl: './contact-details.component.html',
  styleUrls: ['./contact-details.component.scss']
})
export class ContactDetailsComponent implements OnInit, OnDestroy {
  @Input() applicationId: string;
  contactDetailsForm: FormGroup;
  busy: Subscription;
  subscriptions: Subscription[] = [];
  saveFormData: any = {};

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private store: Store<AppState>,
    private route: ActivatedRoute,
    private fb: FormBuilder, public snackBar: MatSnackBar) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    // create entry form and set retrieved values
    this.createForm();

    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication)
      .filter(state => !!state)
      .subscribe(currentApplication => {
        this.contactDetailsForm.patchValue(currentApplication);
        if (currentApplication.isPaid) {
          this.contactDetailsForm.disable();
        }
        this.saveFormData = this.contactDetailsForm.value;
      });
    this.subscriptions.push(sub);

    // const sub2 = this.contactDetailsForm.valueChanges
    //   .pipe(auditTime(10000))
    //   .filter(formData => (JSON.stringify(formData) !== JSON.stringify(this.saveFormData)))
    //   .subscribe(formData => {
    //     this.save();
    //   });
    // this.subscriptions.push(sub2);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  /**
 * Property Details Form
 * */
  createForm() {
    this.contactDetailsForm = this.fb.group({
      id: [''],
      contactpersonfirstname: [''], // Validators.required
      contactpersonlastname: [''],
      contactpersonrole: [''],
      contactpersonemail: ['', Validators.email],
      contactpersonphone: ['']
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.saveFormData) === JSON.stringify(this.contactDetailsForm.value)) {
      return true;
    } else {
      return this.save(true);
    }
  }

  /**
   * Save data in Dynamics
   * */
  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const saveData = this.contactDetailsForm.value;
    const subscription = this.applicationDataService.updateApplication(this.contactDetailsForm.value).subscribe(
      res => {
        saveResult.next(true);
        this.saveFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true) {
          this.snackBar.open('Contact Details have been saved', 'Success', { duration: 2500, panelClass: ['red-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Contact Details', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured saving Contact Details');
      });

    if (showProgress === true) {
      this.busy = subscription;
    }

    return saveResult;
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: AdoxioApplication) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

  /**
   * Check if entry field has an error
   * @param field
   */
  isFieldError(field: string) {
    const isError = !this.contactDetailsForm.get(field).valid && this.contactDetailsForm.get(field).touched;
    return isError;
  }

}
