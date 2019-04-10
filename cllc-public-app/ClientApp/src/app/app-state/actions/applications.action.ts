import { Action } from '@ngrx/store';
import { Application } from '../../models/application.model';

export const APPLICATION = 'APPLICATION';
export const SET_APPLICATION = 'SET_APPLICATION';

export class ApplicationsAction implements Action {
  readonly type = APPLICATION;
}

export class SetApplicationsAction implements Action {
  readonly type = SET_APPLICATION;

  constructor(public payload: Application[]) { }
}

export type Actions =
  ApplicationsAction
  | SetApplicationsAction;
