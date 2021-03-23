import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ApplicationCancellationDialogComponent } from '@components/dashboard/applications-and-licences/applications-and-licences.component';

@Component({
  selector: 'app-starter-checklist',
  templateUrl: './starter-checklist.component.html',
  styleUrls: ['./starter-checklist.component.scss']
})
export class StarterChecklistComponent implements OnInit {
  @Input() showCreateButton = false;

  constructor(
    public dialogRef: MatDialogRef<ApplicationCancellationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

  }

  close(startApp: boolean) {
    this.dialogRef.close(startApp);
  }

  ngOnInit(): void {
  }

}
