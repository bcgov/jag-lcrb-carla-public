import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';

@Component({
  selector: 'app-police-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;
  isNewUser = false;
  dataLoaded = false;

  constructor(private store: Store<AppState>) { }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => this.loadUser(user));
  }

  loadUser(user: User) {
    this.currentUser = user;
    this.isNewUser = this.currentUser.isNewUser;
    this.dataLoaded = true;
  }

}
