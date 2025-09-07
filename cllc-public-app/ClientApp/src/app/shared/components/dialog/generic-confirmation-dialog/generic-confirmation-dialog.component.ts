import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface GenericConfirmationDialogData {
  /**
   * Optional callback function fired when the user clicks "Yes".
   */
  onConfirm?: () => void;
  /**
   * Optional callback function fired when the user clicks "No".
   */
  onCancel?: () => void;
  /**
   * Dialog title.
   */
  title: string;
  /**
   * Dialog message.
   */
  message: string;
  /**
   * Text for the cancel button.
   */
  cancelButtonText: string;
  /**
   * Text for the confirm button.
   *
   * @type {string}
   */
  confirmButtonText: string;
}

/**
 * A general purpose "Are you sure you want to ...?" dialog component.
 *
 * @export
 * @class GenericConfirmationDialogComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-generic-confirmation-dialog',
  templateUrl: './generic-confirmation-dialog.component.html',
  styleUrls: ['./generic-confirmation-dialog.component.scss']
})
export class GenericConfirmationDialogComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<GenericConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GenericConfirmationDialogData
  ) {}

  ngOnInit(): void {}

  handleCancel() {
    this.data.onCancel?.();

    this.dialogRef.close(false);
  }

  handleConfirm() {
    this.data.onConfirm?.();

    this.dialogRef.close(true);
  }
}
