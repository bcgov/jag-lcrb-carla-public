import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AppState } from '@app/app-state/models/app-state';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { Store } from '@ngrx/store';
import { Observable, of } from 'rxjs';
import { ApplicantComponent } from './applicant/applicant.component';
import { EligibilityComponent } from './eligibility/eligibility.component';
import { EventComponent } from './event/event.component';
import { LiquorComponent } from './liquor/liquor.component';
import { SummaryComponent } from './summary/summary.component';
import { Account } from '@models/account.model';
import { ActivatedRoute } from '@angular/router';
import { IndexedDBService } from '@services/indexed-db.service';
import { SepApplication } from '@models/sep-application.model';
import { FormBuilder, FormGroup } from '@angular/forms';

export const SEP_APPLICATION_STEPS = ["applicant", "eligibility", "event", "liquor", "summary"];

@Component({
  selector: 'app-sep-application',
  templateUrl: './sep-application.component.html',
  styleUrls: ['./sep-application.component.scss']
})
export class SepApplicationComponent implements OnInit {
  faCheck = faCheck;
  securityScreeningEnabled: boolean;
  localId: number;
  isFree: boolean = false;
  hasLGApproval: boolean = false;


  stepType: "summary";
  application: SepApplication;
  steps = SEP_APPLICATION_STEPS;
  account: Account;
  step: string;

  get selectedIndex(): number {
    let index = 0;
    if (this.step) {
      index = this.steps.indexOf(this.step);
      if (index === -1) {
        index = 0
      }
    }
    return index;
  }

  constructor(private store: Store<AppState>,
    private db: IndexedDBService,
    private cd: ChangeDetectorRef,
    private route: ActivatedRoute) {
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => this.account = account);
    this.route.paramMap.subscribe(pmap => {
      // if the id is 'new' set it to null ( this will dictate whether the save is a create or an update)
      this.localId = pmap.get('id') === 'new' ? null : parseInt(pmap.get('id'), 10);
      this.step = pmap.get('step');
    });
  }

  ngOnInit() {

    this.getApplication();

  }

  getApplication() {
    if (this.localId) {
      this.db.getSepApplication(this.localId)
        .then(app => {
          this.application = app;
        }, err => {
          console.error(err);
        });
    }
  }

  canActivate(): Observable<boolean> {
    let result: Observable<boolean> = of(true);
    return result;
  }

  isStepCompleted(step: string): boolean {
    let completed = false;
    if (this?.application?.stepsCompleted && step) {
      if (this.application.stepsCompleted.indexOf(step) !== -1) {
        completed = true;
      }
    }
    return completed;
  }

  completeStep(step: string) {
    const steps = this?.application?.stepsCompleted;
    if (steps && step && steps.indexOf(step) == -1) {
      this.application.stepsCompleted.push(step);
    }
    this.cd.detectChanges();
  }

  selectionChange(event) {
  }

}
