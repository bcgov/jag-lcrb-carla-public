import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Contact } from '@models/contact.model';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';
import { AccountDataService } from '@services/account-data.service';
import { Subscription } from 'rxjs';
import { filter, map } from 'rxjs/operators';

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
  ) {}

  ngOnInit() {
    this.subscribeForData();
  }

  private subscribeForData() {
    this.store
      .select((state) => state.currentUserState.currentUser)
      .pipe(filter((s) => !!s))
      .subscribe((user: User) => {
        this.currentUser = user;
      });

    // fetch possible contacts we can assign to.
    this.loadAccountContacts().subscribe((availableContacts) => (this.availableContacts = availableContacts));
  }

  private loadAccountContacts() {
    return this.accountDataService.getCurrentAccountContacts().pipe(
      map((array) =>
        array.map((accountContactData) => {
          return {
            ...accountContactData
          } as Contact;
        })
      )
    );
  }
}
