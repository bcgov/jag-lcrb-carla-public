import { Component, OnInit, Input, Inject } from '@angular/core';
import { MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit {
  title: string;
  body: string;

  constructor(
    public dialogRef: MatDialogRef<ModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.title = data.title;
    this.body = data.body;
  }

  ngOnInit() {
  }

}
// @Component({
//   selector: 'app-application-cancellation-dialog',
//   templateUrl: 'application-cancellation-dialog.html',
// })
// export class ApplicationCancellationDialogComponent {

//   establishmentName: string;
//   applicationName: string;

//   constructor(
//     public dialogRef: MatDialogRef<ApplicationCancellationDialogComponent>,
//     @Inject(MAT_DIALOG_DATA) public data: any) {
//     this.applicationName = data.applicationName;
//     this.establishmentName = data.establishmentName;
//   }

//   close() {
//     this.dialogRef.close(false);
//   }

//   cancel() {
//     this.dialogRef.close(true);
//   }

// }
