import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { IndexDBService } from '@services/index-db.service';
import { StarterChecklistComponent } from '../starter-checklist/starter-checklist.component';

@Component({
  selector: 'app-my-applications',
  templateUrl: './my-applications.component.html',
  styleUrls: ['./my-applications.component.scss']
})
export class MyApplicationsComponent implements OnInit {
  applications: any[];
  displayedColumns = ['status', 'info', 'actions'];
  constructor(private db: IndexDBService,
    private router: Router,
    private dialog: MatDialog) {
   }

  async ngOnInit() {
    this.applications = await this.db.getSepApplications();
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
        if(startApplication){
          this.router.navigateByUrl('/sep/application')
        }
      });
  }

}
