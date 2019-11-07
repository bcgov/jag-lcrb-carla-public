
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { VersionInfo } from '@models/version-info.model';

/***************************************
 * Version Info Dialog
 ***************************************/
@Component({
  selector: 'version-info-dialog',
    templateUrl: './version-info-dialog.component.html',
})
export class VersionInfoDialogComponent {
  public versionInfo: VersionInfo;
  constructor(
      private dialogRef: MatDialogRef<VersionInfoDialogComponent>,
      @Inject(MAT_DIALOG_DATA) public data: any) {
    
    if (data) {
        this.versionInfo = data;
    }    
  }

  close() {
    this.dialogRef.close();
  }
}
