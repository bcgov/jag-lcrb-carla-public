import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { SelectionModel } from '@angular/cdk/collections';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngrx/store';
import { filter, map } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { AccountDataService } from '@services/account-data.service';
import { Contact } from '@models/contact.model';
import { Subscription } from "rxjs";
import { PoliceTableElement } from '../police-table-element';

@Component({
  selector: 'app-my-jobs',
  templateUrl: './my-jobs.component.html',
  styleUrls: ['./my-jobs.component.scss']
})
export class MyJobsComponent implements OnInit {

  // icons
  busy: Subscription;
  // angular material table columns to display
  currentUser: User;
  availableContacts = [];
  dataSourceInProgress = new MatTableDataSource<PoliceTableElement>();
  dataSourcePoliceApproved = new MatTableDataSource<PoliceTableElement>();
  dataSourceIssued = new MatTableDataSource<PoliceTableElement>();

  selectedIndex = 0;
  value: any = {};


  constructor(
    private store: Store<AppState>,
    private sepDataService: SpecialEventsDataService,
    private accountDataService: AccountDataService,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.subscribeForData();
  }

  private subscribeForData() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(filter(s => !!s))
      .subscribe((user: User) => {
        this.currentUser = user;
      });

    // fetch possible contacts we can assign to.
    this.loadAccountContacts()
      .subscribe(availableContacts => this.availableContacts = availableContacts);
   
    // fetch SEP applications waiting for Police Approval
    this.loadSepApplications()
      .subscribe(myApplications => {
        this.dataSourceInProgress.data = myApplications.inProgress;
        this.dataSourcePoliceApproved.data = myApplications.policeApproved;
        this.dataSourceIssued.data = myApplications.issued;        
      });
  }

  private loadAccountContacts()
  {
    return this.accountDataService.getCurrentAccountContacts()
    .pipe(map(array => array.map(accountContactData => {
      return {
        ...accountContactData        
      } as Contact;
    })));
  }
  private loadSepApplications() {
    return this.sepDataService.getPoliceApprovalMySepApplications();
  }

}
