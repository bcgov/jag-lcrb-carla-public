import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { auditTime } from 'rxjs/operators';
import { Observable } from '../../../../../node_modules/rxjs/Observable';
import { Subject } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import * as currentApplicationActions from '../../../app-state/actions/current-application.action';

@Component({
  selector: 'app-property-details',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.scss']
})
export class PropertyDetailsComponent implements OnInit, OnDestroy {
  @Input() applicationId: string;
  propertyDetailsForm: FormGroup;
  busy: Subscription;
  subscriptions: Subscription[] = [];
  saveFormData: any = {};

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private store: Store<AppState>,
    private fb: FormBuilder,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  /**
   *
   * */
  ngOnInit() {
    // create entry form
    this.createForm();
    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication)
      .filter(state => !!state)
      .subscribe(currentApplication => {
        this.propertyDetailsForm.patchValue(currentApplication);
        if (currentApplication.isPaid) {
          this.propertyDetailsForm.disable();
        }
        this.saveFormData = this.propertyDetailsForm.value;
      });
    this.subscriptions.push(sub);

    // const sub2 = this.propertyDetailsForm.valueChanges
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
    this.propertyDetailsForm = this.fb.group({
      id: [''],
      establishmentaddressstreet: [''], // Validators.required
      establishmentaddresscity: [''],
      establishmentaddresspostalcode: [''],
      establishmentparcelid: [''],
      additionalpropertyinformation: ['']
    });


  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.saveFormData) === JSON.stringify(this.propertyDetailsForm.value)) {
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
    const saveData = this.propertyDetailsForm.value;
    const subscription = this.applicationDataService.updateApplication(this.propertyDetailsForm.value).subscribe(
      res => {
        saveResult.next(true);
        this.updateApplicationInStore();
        this.saveFormData = saveData;
        if (showProgress === true) {
          this.snackBar.open('Property Details have been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Property Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured saving Property Details');
      });

    if (showProgress === true) {
      this.busy = subscription;
    }
    return saveResult;
  }

  /**
   * Check if entry field has an error
   * @param field
   */
  isFieldError(field: string) {
    const isError = !this.propertyDetailsForm.get(field).valid && this.propertyDetailsForm.get(field).touched;
    return isError;
  }

}
