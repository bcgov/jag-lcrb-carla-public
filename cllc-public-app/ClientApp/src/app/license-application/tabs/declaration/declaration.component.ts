import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import * as currentApplicationActions from '../../../app-state/actions/current-application.action';
import { AdoxioApplication } from '../../../models/adoxio-application.model';

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
  subscriptions: Subscription[] = [];
  application: any;
  savedFormData: any = {};
  isReadOnly = false;

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private store: Store<AppState>,
    private route: ActivatedRoute,
    public snackBar: MatSnackBar) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication)
      .filter(state => !!state)
      .subscribe(currentApplication => {
        this.signatureagreement = currentApplication.signatureagreement;
        this.authorizedtosubmit = currentApplication.authorizedtosubmit;
        if (currentApplication.isPaid) {
          this.isReadOnly = true;
        }
        this.savedFormData = {
          authorizedtosubmit: currentApplication.authorizedtosubmit,
          signatureagreement: currentApplication.signatureagreement,
        };
      });
    this.subscriptions.push(sub);
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
        this.updateApplicationInStore();
        this.savedFormData = {
          authorizedtosubmit: declarationValues.authorizedtosubmit,
          signatureagreement: declarationValues.signatureagreement,
        };
        if (showProgress === true) {
          this.snackBar.open('Declaration Details have been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
        }
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

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: AdoxioApplication) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

}
