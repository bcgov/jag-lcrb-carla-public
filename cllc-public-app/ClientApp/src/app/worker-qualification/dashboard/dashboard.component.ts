import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { Worker } from '../../models/worker.model';
import { WorkerDataService } from '../../services/worker-data.service.';
import { Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class WorkerDashboardComponent implements OnInit {
  currentUser: User;
  displayedColumns = ['lastUpdated', 'worker', 'status'];
  dataSource: Worker[] = [];
  isNewUser: boolean;
  dataLoaded = false;
  applicationStatus: string;
  // numberOfApplications = 0;
  id: string;
  firstName: string;
  lastName: string;

  busy: Subscription;
  currentApplication: Worker;
  // @ViewChild(MatPaginator) paginator: MatPaginator;
  constructor(
    private userDataService: UserDataService,
    private workerDataService: WorkerDataService,
    private store: Store<AppState>
  ) {
  }

  ngOnInit() {
    this.reloadUser();
  }

  reloadUser() {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        this.currentUser = user;
        this.isNewUser = this.currentUser.isNewUser;
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.contactid) {

          this.busy = this.workerDataService.getWorkerByContactId(this.currentUser.contactid).subscribe(res => {
            this.dataSource = res;
            this.currentApplication = res[0];
            this.setClientSideStatus(this.currentApplication);

            this.applicationStatus = this.getStatus(res);

            const passedApplications = res.filter(i => (<any>i).status === 'Active');

            if (passedApplications.length > 0) {
              this.displayedColumns.push('actions');
            }
          });
        }
      });
  }
  setClientSideStatus(worker: Worker) {
    worker.clientSideStatus = worker.status;
    if (!worker.paymentReceived) {
      worker.clientSideStatus = 'Not Completed';
    } else if (worker.paymentReceived
      && worker.status !== 'Active'
      && worker.status !== 'Withdrawn'
      && worker.status !== 'Rejected'
      && worker.status !== 'Revoked'
      && worker.status !== 'Expired') {
      worker.clientSideStatus = 'Pending Review';
    }
  }

  getStatus(res) {
    // if we've received payment
    if (res[0].status === 'Not Submitted' && this.isNewUser) {
      return 'Not Started';
    }
    return res[0].status;

  }

  getName(res) {
    return;
  }

}
