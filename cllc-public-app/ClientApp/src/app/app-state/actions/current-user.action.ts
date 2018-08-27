import { Action } from '@ngrx/store';
import { User } from '../../models/user.model';

export const CURRENT_USER = 'CURRENT_USER';
export const SET_CURRENT_USER = 'SET_CURRENT_USER';

export class CurrentUserAction implements Action {
  readonly type = CURRENT_USER;
}

export class SetCurrentUserAction implements Action {
  readonly type = SET_CURRENT_USER;

  constructor(public payload: User) { }
}

export type Actions =
CurrentUserAction
  | SetCurrentUserAction;
