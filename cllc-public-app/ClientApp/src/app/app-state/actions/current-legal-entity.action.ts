import { Action } from '@ngrx/store';
import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';


export const  CURRENT_LEGAL_ENTITY = 'CURRENT_LEGAL_ENTITY';
export const  SET_CURRENT_LEGAL_ENTITY = 'SET_CURRENT_LEGAL_ENTITY';

export class CurrentLegalEntityAction implements Action {
  readonly type = CURRENT_LEGAL_ENTITY;
}

export class SetCurrentLegalEntityAction implements Action {
  readonly type = SET_CURRENT_LEGAL_ENTITY;

  constructor(public payload: AdoxioLegalEntity) {}
}

export type Actions =
CurrentLegalEntityAction
| SetCurrentLegalEntityAction;
