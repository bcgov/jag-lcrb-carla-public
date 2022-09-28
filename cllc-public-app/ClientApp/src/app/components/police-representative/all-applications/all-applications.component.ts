import { Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngrx/store';
import { filter, map } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
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
  initialSelection = [];


  constructor(
    private store: Store<AppState>,
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

  }

  private loadAccountContacts() {
    return this.accountDataService.getCurrentAccountContacts()
    .pipe(map(array => array.map(accountContactData => {
      return {
        ...accountContactData        
      } as Contact;
    })));
  }

 

}
