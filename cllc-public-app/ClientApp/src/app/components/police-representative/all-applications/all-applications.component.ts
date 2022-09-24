import { Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngrx/store';
import { filter, map } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { AccountDataService } from '@services/account-data.service';
import { Contact } from '@models/contact.model';
import { PoliceTableElement } from '../police-table-element';
import { forkJoin, Subscription } from 'rxjs';

@Component({
  selector: 'app-sep-all-applications',
  templateUrl: './all-applications.component.html',
  styleUrls: ['./all-applications.component.scss']
})
export class AllApplicationsComponent implements OnInit {

  currentUser: User;
  availableContacts = [];
  busy: Subscription;
  selectedIndex: any;

  // table state
  dataSourceInProgress = new MatTableDataSource<PoliceTableElement>();
  dataSourcePoliceApproved = new MatTableDataSource<PoliceTableElement>();
  dataSourcePoliceDenied = new MatTableDataSource<PoliceTableElement>();
  initialSelection = [];
  inProgressCount: number;
  policeApprovedCount: number;
  policeDeniedCount: number;


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
   
    // fetch SEP applications
    this.busy = this.loadSepApplications()
      .subscribe(allApplications => {
        this.dataSourceInProgress.data = allApplications[0].value;
        this.inProgressCount = allApplications[0].count
        this.dataSourcePoliceApproved.data = allApplications[1].value;
        this.policeApprovedCount = allApplications[1].count
        this.dataSourcePoliceDenied.data = allApplications[2].value;        
        this.policeDeniedCount = allApplications[2].count;
      });

  }

  private loadAccountContacts() {
    return this.accountDataService.getCurrentAccountContacts()
    .pipe(map(array => array.map(accountContactData => {
      return {
        ...accountContactData        
      } as Contact;
    })));
  }

  private loadSepApplications() {
    let pendingReviewApplications = this.sepDataService.getPolicePendingReviewSepApplications(0, 5);

    let approvedApplications = this.sepDataService.getPoliceApprovedSepApplications(0, 5);

    let deniedApplications = this.sepDataService.getPoliceDeniedSepApplications(0, 5);

    return forkJoin([pendingReviewApplications,
      approvedApplications,
      deniedApplications]);
  }

  private loadPendingReviewSepApplications(pageIndex: number = 0, pageSize: number = 10) {
    this.sepDataService.getPolicePendingReviewSepApplications(pageIndex, pageSize)
      .subscribe(data => {
        console.log(data);
        console.log(this.dataSourceInProgress);
        this.dataSourceInProgress = new MatTableDataSource<PoliceTableElement>()
        this.dataSourceInProgress.data = data.value;
        this.inProgressCount = data.count;
        console.log(this.dataSourceInProgress);
      });
  }

  private loadPoliceApprovedSepApplications(pageIndex: number = 0, pageSize: number = 10) {
    this.sepDataService.getPoliceApprovedSepApplications(pageIndex, pageSize)
      .subscribe(data => {
        this.dataSourcePoliceApproved.data = data.value;
        this.policeApprovedCount = data.count;
      })
  }

  private loadPoliceDeniedSepApplications(pageIndex: number = 0, pageSize: number = 10) {
    this.sepDataService.getPoliceDeniedSepApplications(pageIndex, pageSize)
      .subscribe(data => {
        this.dataSourcePoliceDenied.data = data.value;
        this.policeDeniedCount = data.count;
      })
  }

}
