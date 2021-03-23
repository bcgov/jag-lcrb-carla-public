import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';
import { StarterChecklistComponent } from '@components/sep/starter-checklist/starter-checklist.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;

  constructor(public dialog: MatDialog,
    private store: Store<AppState>) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe((user: User) => {
        this.currentUser = user;
      })
  }

  ngOnInit(): void {
  }

  startApplication() {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(StarterChecklistComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe(cancelApplication => {

      });
  }

}
