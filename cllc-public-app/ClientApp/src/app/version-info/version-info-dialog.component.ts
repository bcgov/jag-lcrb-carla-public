
import { Component, OnInit, Renderer2, Inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MatTableDataSource, MatDialog, MatSnackBar, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { isDevMode } from '@angular/core';
import { Store } from '@ngrx/store';
import { filter, takeWhile, map } from 'rxjs/operators';
import { VersionInfo } from '../models/version-info.model';

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
