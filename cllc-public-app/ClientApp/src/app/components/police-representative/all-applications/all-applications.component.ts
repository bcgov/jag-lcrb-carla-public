import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngrx/store';
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
  currentUser: User;

  // table state
  dataSource = new MatTableDataSource<TableElement>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(
    private store: Store<AppState>,
    private sepDataService: SpecialEventsDataService,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.subscribeForData();
  }

  private subscribeForData() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(filter(s => !!s))
      .subscribe((user: User) => {
        this.currentUser = user;
      });

    // fetch SEP applications waiting for Police Approval
    this.loadSepApplications()
      .subscribe(applications => this.dataSource.data = applications);
  }

  private loadSepApplications() {
    return this.sepDataService.getPoliceApprovalSepApplications()
      .pipe(map(array => array.map(sepData => {
        // TODO: Add text labels for numeric status values here (from Dynamics enums)
        return {
          ...sepData,
          eventStatusLabel: 'In Progress',
          typeOfEventLabel: 'Members',
          policeDecisionByLabel: 'Vancouver PoliceUser',
        } as TableElement;
      })));
  }

  isAssigned(sepData: TableElement): boolean {
    // TODO: Implement logic to show appropriate button text when application has been assigned
    return sepData.policeDecisionBy != null;
  }
}
