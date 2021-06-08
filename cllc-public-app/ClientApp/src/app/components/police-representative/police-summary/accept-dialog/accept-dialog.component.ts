import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-accept-dialog',
  templateUrl: './accept-dialog.component.html',
  styleUrls: ['./accept-dialog.component.scss']
})
export class AcceptDialogComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<AcceptDialogComponent>) {
  }
  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close(false);
  }

  accept() {
    this.dialogRef.close(true);
  }

}
