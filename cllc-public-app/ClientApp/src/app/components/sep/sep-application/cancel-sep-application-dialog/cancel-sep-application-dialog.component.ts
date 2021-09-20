import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cancel-sep-application-dialog',
  templateUrl: './cancel-sep-application-dialog.component.html',
  styleUrls: ['./cancel-sep-application-dialog.component.scss']
})
export class CancelSepApplicationDialogComponent implements OnInit {

  reason: string = "Withdrawn by Applicant";

  constructor(public dialogRef: MatDialogRef<CancelSepApplicationDialogComponent>) {
  }
  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close([false]);
  }

  cancel() {
    this.dialogRef.close([true, this.reason]);
  }

}
