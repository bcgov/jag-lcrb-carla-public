import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { AppState } from "@app/app-state/models/app-state";
import { faCheck } from "@fortawesome/free-solid-svg-icons";
import { Store } from "@ngrx/store";
import { Observable, of } from "rxjs";
import { Account } from "@models/account.model";
import { ActivatedRoute } from "@angular/router";
import { IndexedDBService } from "@services/indexed-db.service";
import { SepApplication } from "@models/sep-application.model";
import { environment } from "environments/environment";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { MatSnackBar } from '@angular/material/snack-bar';

export const SEP_APPLICATION_STEPS = ["applicant", "eligibility", "event", "liquor", "summary"];

@Component({
  selector: "app-sep-application",
  templateUrl: "./sep-application.component.html",
  styleUrls: ["./sep-application.component.scss"]
})
export class SepApplicationComponent implements OnInit {
  faCheck = faCheck;
  securityScreeningEnabled: boolean;
  localId: number;
  isFree = false;
  hasLGApproval = false;
  isDevEnv = environment.development;

  stepType: "summary";
  application: SepApplication;
  steps = SEP_APPLICATION_STEPS;
  account: Account;
  step: string;
  savingToAPI: boolean;
  isFormEditable: boolean = false;
  get selectedIndex(): number {
    let index = 0;
    if (this.step) {
      index = this.steps.indexOf(this.step);
      if (index === -1) {
        index = 0;
      }
    }
    return index;
  }

  constructor(private store: Store<AppState>,
    public snackBar: MatSnackBar,
    private db: IndexedDBService,
    private cd: ChangeDetectorRef,
    private sepDataService: SpecialEventsDataService,
    private route: ActivatedRoute) {
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => this.account = account);
    this.route.paramMap.subscribe(pmap => {
      // if the id is 'new' set it to null ( this will dictate whether the save is a create or an update)
      this.localId = pmap.get("id") === "new" ? null : parseInt(pmap.get("id"), 10);
      this.step = pmap.get("step");
    });
  }

  ngOnInit() {
    this.getApplication();
  }

  async getApplication() {
    if (this.localId) {
      await this.db.getSepApplication(this.localId)
        .then(app => {
          const value = JSON.parse(JSON.stringify(app));
          delete value.totalMaximumNumberOfGuests;
          this.application = Object.assign(new SepApplication(), value);
          this.isFormEditable = this.application.eventStatus.toLowerCase() === "draft";     
        }, err => {
          console.error(err);
        });
    }
  }

  canActivate(): Observable<boolean> {
    const result: Observable<boolean> = of(true);
    return result;
  }

  isStepCompleted(step: string): boolean {
    let completed = false;
    const indexOfLastStep = this.steps.indexOf(this?.application?.lastStepCompleted);
    completed = this.steps.indexOf(step) <= indexOfLastStep;
    return completed;
  }

  async saveToAPI(): Promise<void> {
    this.savingToAPI = true;
    try {
      const appData = await this.db.getSepApplication(this.localId);
      if (appData.id) { // do an update (the record exists in dynamics)
      try {
        const result = await this.sepDataService.updateSepApplication({ ...appData, invoiceTrigger: true } as SepApplication, appData.id)
        .toPromise();
        if (result.localId) {
        await this.db.saveSepApplication(result);
        }
      } catch (error) {
        this.snackBar.open("Unable to update application", '', { duration: 2500, panelClass: ['red-snackbar'] });
      }
      } else { // create a new record
      try {
        const result = await this.sepDataService.createSepApplication({ ...appData, invoiceTrigger: true } as SepApplication)
        .toPromise();
        if (result.localId) {
        await this.db.saveSepApplication(result);
        this.localId = result.localId;
        }
      } catch (error) {
        this.snackBar.open("Unable to create application", '', { duration: 2500, panelClass: ['red-snackbar'] });
      }
      }
    } catch (error) {
      console.error("Error retrieving application data:", error);
      this.snackBar.open("Unable to retrieve application data", '', { duration: 2500, panelClass: ['red-snackbar'] });
    } finally {
      this.savingToAPI = false;
    }
  }

  completeStep(step: string, stepper: any, data: SepApplication, saveToApi: boolean) {
    
    let lastStepCompleted = step;
    if (this.application)
    {
      const currentLastStepNumber = this.steps.indexOf(this.application.lastStepCompleted);
      const newLastStepNumber = this.steps.indexOf(step);
      if (newLastStepNumber > currentLastStepNumber)
      {
        this.application.lastStepCompleted = step;
      } 
      else
      {
        lastStepCompleted = this.application.lastStepCompleted;
      }     
    }
    
    data.lastStepCompleted = lastStepCompleted;
    this.saveToDb(data);
    this.cd.detectChanges();
    if (saveToApi) {
      this.saveToAPI().then(_ => { // Save to dynamics on transitions on DEV
        stepper.next();
      });
    } else {
      stepper.next();
    }
  }

  selectionChange(event) {
    this.step = this.steps[event.selectedIndex];
  }

  async saveToDb(data) {
    data.localId = this.localId;
    if (!this.localId) {
      data.dateCreated = new Date();
    }
    this.localId = await this.db.saveSepApplication(data);
    await this.getApplication();
    return this.localId;
  }
}
