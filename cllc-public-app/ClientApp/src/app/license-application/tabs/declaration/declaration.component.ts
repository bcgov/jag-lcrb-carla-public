
import {filter} from 'rxjs/operators';
import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { Subscription ,  Observable ,  Subject } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import * as currentApplicationActions from '../../../app-state/actions/current-application.action';
import { Application } from '../../../models/application.model';

@Component({
  selector: 'app-declaration',
  templateUrl: './declaration.component.html',
  styleUrls: ['./declaration.component.scss']
})
export class DeclarationComponent implements OnInit {
  @Input() applicationId: string;
  authorizedToSubmit: boolean;
  signatureAgreement: boolean;
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
    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication).pipe(
      filter(state => !!state))
      .subscribe(currentApplication => {
        this.signatureAgreement = currentApplication.signatureAgreement;
        this.authorizedToSubmit = currentApplication.authorizedToSubmit;
        if (currentApplication.isPaid) {
          this.isReadOnly = true;
        }
        this.savedFormData = {
          authorizedToSubmit: currentApplication.authorizedToSubmit,
          signatureAgreement: currentApplication.signatureAgreement,
        };
      });
    this.subscriptions.push(sub);
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.signatureAgreement === this.savedFormData.signatureAgreement &&
      this.authorizedToSubmit === this.savedFormData.authorizedToSubmit) {
      return true;
    } else {
      return this.save(true);
    }
  }

  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const declarationValues = {
      id: this.applicationId,
      signatureAgreement: this.signatureAgreement,
      authorizedToSubmit: this.authorizedToSubmit
    };
    const subscription = this.applicationDataService.updateApplication(declarationValues).subscribe(
      res => {
        saveResult.next(true);
        this.updateApplicationInStore();
        this.savedFormData = {
          authorizedTosubmit: declarationValues.authorizedToSubmit,
          signatureAgreement: declarationValues.signatureAgreement,
        };
        if (showProgress === true) {
          this.snackBar.open('Declaration Details have been saved', 'Success', { duration: 2500, panelClass: ['red-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Declaration Details', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured saving Declaration Details');
      });
    if (showProgress === true) {
      this.busy = subscription;
    }
    return saveResult;
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

}
