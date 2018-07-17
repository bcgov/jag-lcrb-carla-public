import { Action } from '@ngrx/store';
import { DynamicsAccount } from '../../models/dynamics-account.model';

export const CURRENT_ACCOUNT = 'CURRENT_ACCOUNT';
export const SET_CURRENT_ACCOUNT = 'SET_CURRENT_ACCOUNT';

export class CurrentAccountAction implements Action {
  readonly type = CURRENT_ACCOUNT;
}

export class SetCurrentAccountAction implements Action {
  readonly type = SET_CURRENT_ACCOUNT;

  constructor(public payload: DynamicsAccount) { }
}

export type Actions =
  CurrentAccountAction
  | SetCurrentAccountAction;
