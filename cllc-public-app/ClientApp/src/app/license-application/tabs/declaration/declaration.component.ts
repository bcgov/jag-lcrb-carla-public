import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';

@Component({
  selector: 'app-declaration',
  templateUrl: './declaration.component.html',
  styleUrls: ['./declaration.component.scss']
})
export class DeclarationComponent implements OnInit {
  @Input() applicationId: string;
  authorizedtosubmit: boolean;
  signatureagreement: boolean;
  busy: Subscription;
  application: any;

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private route: ActivatedRoute,
    public snackBar: MatSnackBar) {
      this.applicationId =  this.route.parent.snapshot.params.applicationId;
     }

  ngOnInit() {
    this.getApplication();
  }

  getApplication() {
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        // TODO add to autorest
        // this.authorizedtosubmit = data.authorizedtosubmit;
        this.signatureagreement = data.signatureagreement;
      },
      err => {
        this.snackBar.open('Error getting Declaration Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured getting Declaration Details');
      }
    );
  }

  save() {
    const declarationValues = {
      id: this.applicationId,
      signatureagreement: this.signatureagreement,
      authorizedtosubmit: this.authorizedtosubmit
    };
    this.applicationDataService.updateApplication(declarationValues).subscribe(
      res => {
        // console.log("Application updated:", res.json());
        // this.snackBar.open('Declaration Details have been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
      },
      err => {
        this.snackBar.open('Error saving Declaration Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured saving Declaration Details');
      });
  }

}
