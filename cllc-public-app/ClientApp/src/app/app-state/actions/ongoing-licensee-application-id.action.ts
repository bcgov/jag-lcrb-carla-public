import { Action } from '@ngrx/store';


export const ONGOING_LICENSEE_APPLICATION_ID = 'ONGOING_LICENSEE_APPLICATION_ID';
export const SET_ONGOING_LICENSEE_APPLICATION_ID = 'SET_ONGOING_LICENSEE_APPLICATION_ID';

export class OngoingLicenseeApplicationIdAction implements Action {
  readonly type = ONGOING_LICENSEE_APPLICATION_ID;
}

export class SetOngoingLicenseeApplicationIdAction implements Action {
  readonly type = SET_ONGOING_LICENSEE_APPLICATION_ID;

  constructor(public payload: string) { }
}

export type Actions =
OngoingLicenseeApplicationIdAction
  | SetOngoingLicenseeApplicationIdAction;
