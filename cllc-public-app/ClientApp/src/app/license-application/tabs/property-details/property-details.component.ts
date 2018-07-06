import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-property-details',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.scss']
})
export class PropertyDetailsComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;
  propertyDetailsForm: FormGroup;
  busy: Subscription;
  
  constructor(private applicationDataService: AdoxioApplicationDataService, private fb: FormBuilder,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    //this.applicationId = route.snapshot.params.applicationId;
  }

  /**
   *
   * */
  ngOnInit() {
    //create entry form
    this.createForm();
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        let data = res.json();
        this.propertyDetailsForm.patchValue(data);
      },
      err => {
        this.snackBar.open('Error getting Property Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log("Error occured getting Property Details");
      }
    );
  }

  /**
   * Property Details Form
   * */
  createForm() {
    this.propertyDetailsForm = this.fb.group({
      id: [''],
      establishmentaddressstreet: [''],//Validators.required
      establishmentaddresscity: [''],
      establishmentaddresspostalcode: [''],
      establishmentparcelid: [''],
      additionalpropertyinformation: ['']
    });
  }

  /**
   * Save data in Dynamics
   * */
  save() {
    //console.log('propertyDetailsForm valid, value: ', this.propertyDetailsForm.valid, this.propertyDetailsForm.value);

    if (this.propertyDetailsForm.valid) {
      this.busy = this.applicationDataService.updateApplication(this.propertyDetailsForm.value).subscribe(
        res => {
          //console.log("Application updated:", res.json());
          this.snackBar.open('Property Details have been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
        },
        err => {
          this.snackBar.open('Error saving Property Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
          console.log("Error occured saving Property Details");
        });
    } else {
      Object.keys(this.propertyDetailsForm.controls).forEach(field => {
        const control = this.propertyDetailsForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
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
