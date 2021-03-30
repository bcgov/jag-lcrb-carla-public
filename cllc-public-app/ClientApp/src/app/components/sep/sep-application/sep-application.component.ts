import { Component, OnInit, ViewChild } from '@angular/core';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { Observable, of } from 'rxjs';
import { ApplicantComponent } from './applicant/applicant.component';
import { EligibilityComponent } from './eligibility/eligibility.component';
import { EventComponent } from './event/event.component';
import { LiquorComponent } from './liquor/liquor.component';
import { SummaryComponent } from './summary/summary.component';

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
  @ViewChild("event")
  applicationComponent: EventComponent;
  @ViewChild("liquor")
  dynamicApplicationComponent: LiquorComponent;
  stepType: "summary";
  application: SummaryComponent;
  steps = ["applicant", "eligibility", "event", "liquor", "summary"];

  constructor() {
  }

  ngOnInit() {

  }

  canDeactivate(): Observable<boolean> {
    // let result: Observable<boolean> = of(true);
    // if (this.accountProfileComponent) {
    //   result = this.accountProfileComponent.canDeactivate();
    // }
    // if (this.licenseeChangesComponent) {
    //   result = this.licenseeChangesComponent.canDeactivate();
    // }
    // if (this.applicationComponent) {
    //   result = this.applicationComponent.canDeactivate();
    // }
    // if (this.dynamicApplicationComponent) {
    //   result = this.dynamicApplicationComponent.canDeactivate();
    // }
    // return result;
    return of(true);
  }


  selectionChange(event) {

  }

}
