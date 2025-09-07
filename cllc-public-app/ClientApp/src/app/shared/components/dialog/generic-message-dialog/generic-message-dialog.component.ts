import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface GenericMessageDialogData {
  /**
   * Optional callback function fired when the user closes the dialog.
   */
  onClose?: () => void;
  /**
   * Dialog title.
   */
  title: string;
  /**
   * Dialog message.
   */
  message: string;
  /**
   * Text for the close button.
   *
   * @type {string}
   */
  closeButtonText: string;
}

/**
 * A general purpose dialog component for displaying a message to the user, with a single button to close the dialog.
 *
 * @export
 * @class GenericMessageDialogComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-generic-message-dialog',
  templateUrl: './generic-message-dialog.component.html',
  styleUrls: ['./generic-message-dialog.component.scss']
})
export class GenericMessageDialogComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<GenericMessageDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GenericMessageDialogData
  ) {}

  ngOnInit(): void {}

  handleClose() {
    this.data.onClose?.();

    this.dialogRef.close(true);
  }
}
