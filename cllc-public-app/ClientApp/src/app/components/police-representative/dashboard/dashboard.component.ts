import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';

@Component({
  selector: 'app-police-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;
  isNewUser = false;
  dataLoaded = false;
  userLoaded = false;

  assignedJobsInProgress : number;
  assignedJobsInProgressWithExceptions : number;
  assignedApplicationsIssued : number;

  constructor(private store: Store<AppState>,
    private sepDataService: SpecialEventsDataService,) { }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => this.loadUser(user));

      this.sepDataService.getPoliceHome()
      .subscribe(data => {
        this.assignedJobsInProgress = data.assignedJobsInProgress;
        this.assignedJobsInProgressWithExceptions = data.assignedJobsInProgressWithExceptions;
        this.assignedApplicationsIssued = data.assignedApplicationsIssued;   
        this.dataLoaded = true;     
      });
      
  }

  loadUser(user: User) {
    this.currentUser = user;
    this.isNewUser = this.currentUser.isNewUser;
    this.userLoaded = true;
  }

}
