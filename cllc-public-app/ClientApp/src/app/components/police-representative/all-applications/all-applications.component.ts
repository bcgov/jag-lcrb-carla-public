import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { SepApplicationSummary } from '@models/sep-application-summary.model';
import { User } from '@models/user.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';

// Show text labels instead of numeric enum values in the table; e.g. Status = "In Progress" vs. 100,000,001
interface TableElement extends SepApplicationSummary {
  eventStatusLabel?: string;
  policeDecisionByLabel?: string;
  typeOfEventLabel?: string;
}

@Component({
  selector: 'app-sep-all-applications',
  templateUrl: './all-applications.component.html',
  styleUrls: ['./all-applications.component.scss']
})
export class AllApplicationsComponent implements OnInit {
  // icons

  // angular material table columns to display
  columnsToDisplay = ['dateSubmitted', 'eventName', 'eventStartDate', 'eventStatusLabel', 'policeDecisionByLabel', 'maximumNumberOfGuests', 'typeOfEventLabel', 'actions'];

  // component state
  currentUser: User;
  dataSource$: Observable<TableElement[]>;

  constructor(
    private store: Store<AppState>,
    private sepDataService: SpecialEventsDataService,
    private fb: FormBuilder,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.subscribeForData();
  }

  private subscribeForData() {
    // fetch SEP applications waiting for Police Approval
    this.dataSource$ = this.loadSepApplications();

    this.store.select(state => state.currentUserState.currentUser)
      .pipe(filter(s => !!s))
      .subscribe((user: User) => {
        this.currentUser = user;
      });
  }

  private loadSepApplications() {
    return this.sepDataService.getPoliceApprovalSepApplications()
      .pipe(map(array => array.map(sepData => {
        // TODO: Add text labels for numeric status values here
        return {
          ...sepData,
          eventStatusLabel: 'In Progress',
          typeOfEventLabel: 'Members',
          policeDecisionByLabel: 'Vancouver PoliceUser',
        } as TableElement;
      })));
  }
}
