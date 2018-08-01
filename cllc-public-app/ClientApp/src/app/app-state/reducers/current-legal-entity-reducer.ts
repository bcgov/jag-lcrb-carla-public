import { Action } from '@ngrx/store';
import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';
import * as CurrentLegalEntityActions from '../actions/current-legal-entity.action';
import { CurrentLegalEntityState } from '../models/app-state';

// Section 1
const initialState: CurrentLegalEntityState = { currentLegalEntity: null };

// Section 2
export function reducer(state: CurrentLegalEntityState = initialState, action: CurrentLegalEntityActions.Actions): CurrentLegalEntityState {

    // Section 3
    switch (action.type) {
        case CurrentLegalEntityActions.CURRENT_LEGAL_ENTITY:
            return { ...state };
        case CurrentLegalEntityActions.SET_CURRENT_LEGAL_ENTITY:
            return { currentLegalEntity: action.payload };
        default:
            return state;
    }
}
