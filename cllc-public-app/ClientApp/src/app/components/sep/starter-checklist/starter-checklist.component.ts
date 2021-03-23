import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-starter-checklist',
  templateUrl: './starter-checklist.component.html',
  styleUrls: ['./starter-checklist.component.scss']
})
export class StarterChecklistComponent implements OnInit {
  showCreateButton = false;

  constructor(
    public dialogRef: MatDialogRef<StarterChecklistComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
      this.showCreateButton = data.showStartApp;

  }

  close(startApp: boolean) {
    this.dialogRef.close(startApp);
  }

  ngOnInit(): void {
  }

}
