import { Action } from '@ngrx/store';

export const SET_INDIGENOUS_NATION_MODE = 'SET_INDIGENOUS_NATION_MODE';


export class SetIndigenousNationModeAction implements Action {
  readonly type = SET_INDIGENOUS_NATION_MODE;

  constructor(public payload: boolean) { }
}

export type Actions = SetIndigenousNationModeAction;
