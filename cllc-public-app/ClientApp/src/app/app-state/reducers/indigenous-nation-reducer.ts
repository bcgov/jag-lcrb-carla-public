import { Action } from '@ngrx/store';
import * as AppStateActions from '../actions/app-state.action';
import { IndigenousNationState } from '../models/app-state';

// Section 1
const initialState: IndigenousNationState = { indigenousNationModeActive: false };

// Section 2
export function reducer(state: IndigenousNationState = initialState, action: AppStateActions.Actions): IndigenousNationState {

    // Section 3
    switch (action.type) {
        case AppStateActions.SET_INDIGENOUS_NATION_MODE:
            return { indigenousNationModeActive: action.payload };
        default:
            return state;
    }
}
