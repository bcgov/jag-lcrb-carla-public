import { Action } from '@ngrx/store';

export const SET_CURRENT_BUSINESS_TYPE = 'SET_CURRENT_BUSINESS_TYPE';
export const SET_CURRENT_ACCOUNT_ID = 'SET_CURRENT_ACCOUNT_ID';
export const SET_CURRENT_LEGALENTITY_ID = 'SET_CURRENT_LEGALENTITY_ID';



/**
 * Every action is comprised of at least a type and an optional
 * payload. Expressing actions as classes enables powerful
 * type checking in reducer functions.
 *
 * See Discriminated Unions: https://www.typescriptlang.org/docs/handbook/advanced-types.html#discriminated-unions
 */
export class SetCurrentBusinessTypeAction implements Action {
  readonly type = SET_CURRENT_BUSINESS_TYPE;

  constructor(public payload: string) { }
}

export class SetCurrentAccountIdAction implements Action {
  readonly type = SET_CURRENT_ACCOUNT_ID;

  constructor(public payload: string) { }
}
export class SetCurrentLegalEntityIdAction implements Action {
  readonly type = SET_CURRENT_LEGALENTITY_ID;

  constructor(public payload: string) { }
}





/**
 * Export a type alias of all actions in this action group
 * so that reducers can easily compose action types
 */
export type Actions
  = SetCurrentBusinessTypeAction
  | SetCurrentLegalEntityIdAction
  | SetCurrentAccountIdAction;