import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { auditTime } from 'rxjs/operators';
import { Observable } from '../../../../../node_modules/rxjs/Observable';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-property-details',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.scss']
})
export class PropertyDetailsComponent implements OnInit {
  @Input() applicationId: string;
  propertyDetailsForm: FormGroup;
  busy: Subscription;
  saveFormData: any = {};

  constructor(private applicationDataService: AdoxioApplicationDataService, private fb: FormBuilder,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  /**
   *
   * */
  ngOnInit() {
    // create entry form
    this.createForm();
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        this.propertyDetailsForm.patchValue(data);
        this.saveFormData = this.propertyDetailsForm.value;
      },
      err => {
        this.snackBar.open('Error getting Property Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured getting Property Details');
      }
    );
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

    this.propertyDetailsForm.valueChanges
      .pipe(auditTime(10000)).subscribe(data => {
        this.save();
      });
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
