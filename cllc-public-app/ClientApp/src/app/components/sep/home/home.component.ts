import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';

@Component({
  selector: 'app-sep-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class SepHomeComponent implements OnInit {
  currentUser: User;
  dataLoaded = false;

  constructor(private store: Store<AppState>) { }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => this.loadUser(user));
  }

  loadUser(user: User) {
    this.currentUser = user;
    this.dataLoaded = true;
  }

}
