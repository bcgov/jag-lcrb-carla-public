import { Action } from '@ngrx/store';
import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';
import * as LegalEntityActions from '../actions/legal-entities.action';
import { LegalEntitiesState } from '../models/app-state';

// Section 1
const initialState: LegalEntitiesState = { legalEntities: [] };

// Section 2
export function reducer(state: LegalEntitiesState = initialState, action: LegalEntityActions.Actions): LegalEntitiesState {

    // Section 3
    switch (action.type) {
        case LegalEntityActions.LEGAL_ENTITIES:
            return {...state };
        case LegalEntityActions.SET_LEGAL_ENTITIES:
            return { legalEntities: action.payload };
        default:
            return state;
    }
}
