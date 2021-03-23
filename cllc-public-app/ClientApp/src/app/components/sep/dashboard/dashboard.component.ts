import { Component, OnInit } from '@angular/core';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;

  constructor(private store: Store<AppState>) {
    store.select(state => state.currentUserState.currentUser)
    .subscribe((user: User) =>{
      this.currentUser = user;
    })
   }

  ngOnInit(): void {
  }

}
