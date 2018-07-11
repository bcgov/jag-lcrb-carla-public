import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-store-information',
  templateUrl: './store-information.component.html',
  styleUrls: ['./store-information.component.scss']
})
export class StoreInformationComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;
  storeInformationForm: FormGroup;
  busy: Subscription;

  constructor(private applicationDataService: AdoxioApplicationDataService, private fb: FormBuilder,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {

    //this.applicationId = route.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.createForm();
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        let data = res.json();
        this.storeInformationForm.patchValue(data);
      },
      err => {
        console.log("Error occured");
      }
    );
  }

  createForm() {
    this.storeInformationForm = this.fb.group({
      id: [''],
      establishmentName: [''],//Validators.required
    });
  }

  save() {
    //console.log('storeInformationForm valid, value: ', this.storeInformationForm.valid, this.storeInformationForm.value);
    if (this.storeInformationForm.valid) {
      this.busy = this.applicationDataService.updateApplication(this.storeInformationForm.value).subscribe(
        res => {
          //console.log("Application updated:", res.json());
          this.snackBar.open('Store Information has been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
        },
        err => {
          this.snackBar.open('Error saving Store Information', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
          console.log("Error occured");
        });
    } else {
      Object.keys(this.storeInformationForm.controls).forEach(field => {
        const control = this.storeInformationForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  isFieldError(field: string) {
    const isError = !this.storeInformationForm.get(field).valid && this.storeInformationForm.get(field).touched;
    return isError;
  }

}
