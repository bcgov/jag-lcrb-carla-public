import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { ApplicationCancellationDialogComponent } from '@components/dashboard/applications-and-licences/applications-and-licences.component';
import { SepApplication } from '@models/sep-application.model';
import { User } from '@models/user.model';
import { Store } from '@ngrx/store';
import { IndexDBService } from '@services/index-db.service';
import { takeWhile } from 'rxjs/operators';
import { StarterChecklistComponent } from '../starter-checklist/starter-checklist.component';
import { faEdit } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-my-applications',
  templateUrl: './my-applications.component.html',
  styleUrls: ['./my-applications.component.scss']
})
export class MyApplicationsComponent implements OnInit {
  applications: any[];
  displayedColumns = ['status', 'info', 'actions'];
  currentUser: User;
  faEdit = faEdit;

  constructor(private store: Store<AppState>,
    private db: IndexDBService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe((user: User) => {
        this.currentUser = user;
      })
  }

  async ngOnInit() {
    await this.getApplications();
  }

  async getApplications() {
    let apps = await this.db.applications.toArray();
    apps = apps.sort((a, b) => {
      var dateA = new Date(a.dateCreated).getTime();
      var dateB = new Date(b.dateCreated).getTime();
      return dateA > dateB ? -1 : 1;
    });
    this.applications = apps;
  }

  startApplication() {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      data: {
        showStartApp: true
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(StarterChecklistComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe((startApplication: boolean) => {
        if (startApplication) {
          this.router.navigateByUrl('/sep/application/new/applicant')
        }
      });
  }

  /**
   *
   * @param applicationId
   * @param establishmentName
   * @param applicationName
   */
  cancelApplication(applicationId: string, establishmentName: string, applicationName: string) {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "400px",
      height: "200px",
      data: {
        establishmentName: establishmentName,
        applicationName: applicationName
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ApplicationCancellationDialogComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe(async (cancelApplication) => {
        if (cancelApplication) {
          this.db.deleteSepApplication(parseInt(applicationId, 10));
          await this.getApplications()
        }
      });

  }

  async cloneApplication(app: SepApplication) {
    let newId = await this.db.addSepApplication({
      ...app,
      id: undefined,
      dateAgreedToTnC: undefined,
      agreeToTnC: false,
      dateCreated: new Date()
    });
    this.router.navigateByUrl(`/sep/application/${newId}/applicant`)
  }


}
