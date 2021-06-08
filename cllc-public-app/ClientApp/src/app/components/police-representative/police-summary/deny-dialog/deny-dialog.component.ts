import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-deny-dialog',
  templateUrl: './deny-dialog.component.html',
  styleUrls: ['./deny-dialog.component.scss']
})
export class DenyDialogComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<DenyDialogComponent>) {
  }
  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close(false);
  }

  cancel() {
    this.dialogRef.close(true);
  }

}
