import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-declaration',
  templateUrl: './declaration.component.html',
  styleUrls: ['./declaration.component.scss']
})
export class DeclarationComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;
  authorizedtosubmit: boolean;
  signatureagreement: boolean;
  busy: Subscription;
  application: any;

  constructor(private applicationDataService: AdoxioApplicationDataService, public snackBar: MatSnackBar) { }

  ngOnInit() {
    this.getApplication();
  }

  getApplication() {
    this.busy = this.applicationDataService.getApplication(this.applicationId).subscribe(
      res => {
        let data = res.json();
        //TODO add to autorest
        //this.authorizedtosubmit = data.authorizedtosubmit;
        this.signatureagreement = data.signatureagreement;
      },
      err => {
        this.snackBar.open('Error getting Declaration Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log("Error occured getting Declaration Details");
      }
    );
  }

  //save() {
  //  this.busy = this.applicationDataService.updateApplication(this.propertyDetailsForm.value).subscribe(
  //    res => {
  //      //console.log("Application updated:", res.json());
  //      this.snackBar.open('Property Details have been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
  //    },
  //    err => {
  //      this.snackBar.open('Error saving Property Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
  //      console.log("Error occured saving Property Details");
  //    });
  //}

}
