import { Action } from '@ngrx/store';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';

export const LEGAL_ENTITIES = 'LEGAL_ENTITIES';
export const SET_LEGAL_ENTITIES = 'SET_LEGAL_ENTITIES';

export class LegalEntitiesAction implements Action {
    readonly type = LEGAL_ENTITIES;
}

export class SetLegalEntitiesAction implements Action {
    readonly type = SET_LEGAL_ENTITIES;

    constructor(public payload: AdoxioLegalEntity[]) { }
}

export type Actions =
    LegalEntitiesAction
    | SetLegalEntitiesAction;
