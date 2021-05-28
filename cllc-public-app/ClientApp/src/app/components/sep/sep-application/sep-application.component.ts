import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AppState } from '@app/app-state/models/app-state';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { Store } from '@ngrx/store';
import { Observable, of } from 'rxjs';
import { Account } from '@models/account.model';
import { ActivatedRoute } from '@angular/router';
import { IndexedDBService } from '@services/indexed-db.service';
import { SepApplication } from '@models/sep-application.model';
import { environment } from 'environments/environment';
import { SpecialEventsDataService } from '@services/special-events-data.service';

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
  isDevEnv = environment.development;

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
    private sepDataService: SpecialEventsDataService,
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

  async getApplication() {
    if (this.localId) {
      await this.db.getSepApplication(this.localId)
        .then(app => {
          // make sure the steps completed array is setup
          app.stepsCompleted = app.stepsCompleted || [];
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


  async saveToAPI(): Promise<void> {
    let appData = await this.db.getSepApplication(this.localId);
    if (appData.id) { // do an update ( the record exists in dynamics)
      let result = await this.sepDataService.updateSepApplication({ ...appData, invoiceTrigger: 1 }, appData.id)
        .toPromise();
      if (result.localId) {
        await this.db.applications.update(result.localId, result);
      }
    } else {
      let result = await this.sepDataService.createSepApplication({ ...appData, invoiceTrigger: 1 })
        .toPromise();
      if (result.localId) {
        await this.db.applications.update(result.localId, result);
        this.localId = result.localId;
      }
    }
  }


  completeStep(step: string, stepper: any, data: SepApplication) {
    const steps = this?.application?.stepsCompleted;
    if (steps && step && steps.indexOf(step) == -1) {
      this.application.stepsCompleted.push(step);
    }
    this.saveToDb(data);
    this.cd.detectChanges();
    if (environment.development) {
      this.saveToAPI().then(_ => { // Save to dynamics on transitions on DEV
        stepper.next();
      });
    } else {
      stepper.next();
    }
  }

  selectionChange(event) {
  }

  async saveToDb(data) {
    let localId: number = null;
    if (data.localId) {
      await this.db.applications.update(data.localId, data);
    } else {
      data.dateCreated = new Date();
      this.localId = await this.db.saveSepApplication(data);
    }
    await this.getApplication();
    return localId;
  }
}
