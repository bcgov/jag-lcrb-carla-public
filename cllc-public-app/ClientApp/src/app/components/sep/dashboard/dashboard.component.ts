import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { filter } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { Account } from '@models/account.model';
import { StarterChecklistComponent } from '@components/sep/starter-checklist/starter-checklist.component';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';

@Component({
  selector: 'app-sep-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;
  isNewUser = false;
  dataLoaded = false;

  constructor(public dialog: MatDialog,
    private db: IndexedDBService,
    private router: Router,
    private store: Store<AppState>) {
  }

  ngOnInit(): void {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => this.loadUser(user));
  }

  loadUser(user: User) {
    this.currentUser = user;
    this.isNewUser = this.currentUser.isNewUser;
    this.dataLoaded = true;
  }

  startApplication() {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "600px",
      data: {
        showStartApp: true
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(StarterChecklistComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe((startApplication: boolean) => {
        if (startApplication) {
          const data = {
            dateCreated: new Date(),
            eventStatus: "Draft"
          } as SepApplication;

          this.db.saveSepApplication(data)
          .then(localId => {
            this.router.navigateByUrl(`/sep/application/${localId}/applicant`);
          });

        }
      });
  }
}
