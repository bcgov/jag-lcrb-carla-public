import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { PolicyDocumentComponent } from '../../policy-document/policy-document.component';
import { MatDialogRef, MatDialog } from '@angular/material';

@Component({
  selector: 'app-worker-home',
  templateUrl: './worker-home.component.html',
  styleUrls: ['./worker-home.component.scss']
})
export class WorkerHomeComponent implements OnInit, AfterViewInit {

  policySlug = 'worker-qualification-home';
  @ViewChild('policyDocs') policyDocs: PolicyDocumentComponent;
  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    this.policyDocs.setSlug(this.policySlug);
  }


  openRegisterDialog() {
    // set dialogConfig settings
    const dialogConfig: any = {
      autoFocus: true,
      disableClose: false,
      maxWidth: '500px',
      data: {
      }
    };


    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(WorkerHomeDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        if (formData === 'login') {
          window.location.href = '/bcservice';
        } else if (formData === 'create-account') {
          window.location.href = 'https://www2.gov.bc.ca/gov/content/governments/government-id/bc-services-card/login-with-card';
        }
      }
    );
  }

}



/***************************************
 * Confirm registration Dialog
 ***************************************/
@Component({
  selector: 'app-register-dialog',
  templateUrl: 'register-dialog.html',
})
export class WorkerHomeDialogComponent {

  constructor(public dialogRef: MatDialogRef<WorkerHomeDialogComponent>) {
  }

  register() {
  }
}
