import { Action } from '@ngrx/store';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { LegalEntity } from '@models/legal-entity.model';

export const LEGAL_ENTITIES = 'LEGAL_ENTITIES';
export const SET_LEGAL_ENTITIES = 'SET_LEGAL_ENTITIES';

export class LegalEntitiesAction implements Action {
    readonly type = LEGAL_ENTITIES;
}

export class SetLegalEntitiesAction implements Action {
    readonly type = SET_LEGAL_ENTITIES;

    constructor(public payload: LegalEntity[]) { }
}

export type Actions =
    LegalEntitiesAction
    | SetLegalEntitiesAction;
