import { Action } from '@ngrx/store';
import { FileUploadSet } from '../../models/file-upload-set.model';

export const SET_FILE_UPLOADS = 'SET_FILE_UPLOADS';
export const CLEAR_FILE_UPLOADS = 'CLEAR_FILE_UPLOADS';

export class SetFileUploadsAction implements Action {
  readonly type = SET_FILE_UPLOADS;

  constructor(public payload: FileUploadSet) { }
}

export class ClearFileUploadsAction implements Action {
  readonly type = CLEAR_FILE_UPLOADS;

  constructor(public payload: string) { }
}

export type Actions =
SetFileUploadsAction
| ClearFileUploadsAction;
