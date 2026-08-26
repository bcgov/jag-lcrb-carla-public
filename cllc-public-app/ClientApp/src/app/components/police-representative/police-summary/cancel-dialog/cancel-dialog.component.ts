import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cancel-dialog',
  templateUrl: './cancel-dialog.component.html',
  styleUrls: ['./cancel-dialog.component.scss']
})
export class CancelDialogComponent implements OnInit {
  reason: string;

  constructor(public dialogRef: MatDialogRef<CancelDialogComponent>) {}
  ngOnInit(): void {}

  close() {
    this.dialogRef.close([false]);
  }

  cancel() {
    this.dialogRef.close([true, this.reason]);
  }
}
