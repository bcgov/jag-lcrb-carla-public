import { Action } from '@ngrx/store';
import { AdoxioApplication } from '../../models/adoxio-application.model';

export const CURRENT_APPLICATION = 'CURRENT_APPLICATION';
export const SET_CURRENT_APPLICATION = 'SET_CURRENT_APPLICATION';

export class CurrentApplicationAction implements Action {
  readonly type = CURRENT_APPLICATION;
}

export class SetCurrentApplicationAction implements Action {
  readonly type = SET_CURRENT_APPLICATION;

  constructor(public payload: AdoxioApplication) { }
}

export type Actions =
  CurrentApplicationAction
  | SetCurrentApplicationAction;
