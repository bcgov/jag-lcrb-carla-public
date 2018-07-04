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

  applicationId: string;
  propertyDetailsForm: FormGroup;
  busy: Subscription;
  
  constructor(private applicationDataService: AdoxioApplicationDataService, private fb: FormBuilder,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {

    this.applicationId = route.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.createForm();
    // get application data, display form
    this.busy = this.applicationDataService.getApplication(this.applicationId).subscribe(
      res => {
        let data = res.json();
        this.propertyDetailsForm.patchValue(data);
      },
      err => {
        console.log("Error occured");
      }
    );
  }

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
          console.log("Error occured");
        });
    } else {
      Object.keys(this.propertyDetailsForm.controls).forEach(field => {
        const control = this.propertyDetailsForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  isFieldError(field: string) {
    const isError = !this.propertyDetailsForm.get(field).valid && this.propertyDetailsForm.get(field).touched;
    return isError;
  }

}
