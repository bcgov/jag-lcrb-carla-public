import { Component, OnInit, ViewChild } from '@angular/core';
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
import { IndexDBService } from '@services/index-db.service';

@Component({
  selector: 'app-sep-application',
  templateUrl: './sep-application.component.html',
  styleUrls: ['./sep-application.component.scss']
})
export class SepApplicationComponent implements OnInit {
  faCheck = faCheck;
  securityScreeningEnabled: boolean;
  applicationId: string;
  isFree: boolean = false;
  hasLGApproval: boolean = false;

  @ViewChild("applicant")
  accountProfileComponent: ApplicantComponent;
  @ViewChild("eligibility")
  licenseeChangesComponent: EligibilityComponent;
  @ViewChild("event") applicationComponent: EventComponent;
  @ViewChild("liquor") dynamicApplicationComponent: LiquorComponent;
  stepType: "summary";
  application: any;
  steps = ["applicant", "eligibility", "event", "liquor", "summary"];
  account: Account;

  constructor(private store: Store<AppState>,
    private db: IndexDBService,
    private route: ActivatedRoute) {
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => this.account = account);
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('id'));
  }

  ngOnInit() {
    if (this.applicationId) {
      this.db.getSepApplication(parseInt(this.applicationId, 10))
        .then(app => {
          this.application = app;
        }, err => {
          console.error(err);
        });
    }
  }

  canDeactivate(): Observable<boolean> {
    let result: Observable<boolean> = of(true);
    return result;
  }

  selectionChange(event) {
  }

}
