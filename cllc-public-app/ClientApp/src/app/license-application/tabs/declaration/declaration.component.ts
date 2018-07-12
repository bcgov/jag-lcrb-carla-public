import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
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
  savedFormData: any = {};

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private route: ActivatedRoute,
    public snackBar: MatSnackBar) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.getApplication();
  }

  getApplication() {
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        // TODO add to autorest
        this.authorizedtosubmit = data.authorizedtosubmit;
        this.signatureagreement = data.signatureagreement;
        this.savedFormData = {
          authorizedtosubmit: data.authorizedtosubmit,
          signatureagreement: data.signatureagreement,
        };

      },
      err => {
        this.snackBar.open('Error getting Declaration Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured getting Declaration Details');
      }
    );
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.signatureagreement === this.savedFormData.signatureagreement &&
      this.authorizedtosubmit === this.savedFormData.authorizedtosubmit) {
      return true;
    } else {
      return this.save(true);
    }
  }

  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const declarationValues = {
      id: this.applicationId,
      signatureagreement: this.signatureagreement,
      authorizedtosubmit: this.authorizedtosubmit
    };
    const subscription = this.applicationDataService.updateApplication(declarationValues).subscribe(
      res => {
        saveResult.next(true);
        this.savedFormData = {
          authorizedtosubmit: declarationValues.authorizedtosubmit,
          signatureagreement: declarationValues.signatureagreement,
        };
        // this.snackBar.open('Declaration Details have been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Declaration Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured saving Declaration Details');
      });
    if (showProgress === true) {
      this.busy = subscription;
    }
    return saveResult;
  }

}
