import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { Worker } from '../../models/worker.model';
import { WorkerDataService } from '../../services/worker-data.service.';
import { Subscription } from 'rxjs';


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
    private workerDataService: WorkerDataService
  ) {
  }

  ngOnInit() {
    this.reloadUser();
  }

  reloadUser() {
    this.busy = this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
        this.isNewUser = this.currentUser.isNewUser;
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.contactid) {

          this.busy = this.workerDataService.getWorkerByContactId(this.currentUser.contactid).subscribe(res => {
            this.dataSource = res;
            this.currentApplication = res[0];
            this.setClientSideStatus(this.currentApplication);
            // this.numberOfApplications = res.length;
            // if(this.numberOfApplications < 2) {}
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
