import {
    ActionReducerMap,
    createSelector,
    createFeatureSelector,
    ActionReducer,
    MetaReducer,
  } from '@ngrx/store';
import * as appStateActions from '../actions/app-state'

export interface AppState {
    currentBusinessType: string;
    currentAccountId: string;
    currentLegalEntityId: string;
}

export interface State{
    appState: AppState;
}


export function appStateReducer(state = <AppState>{}, action: appStateActions.Actions): AppState {
    switch (action.type) {
        case appStateActions.SET_CURRENT_BUSINESS_TYPE: {
            state.currentBusinessType = action.payload;
            return { ...state };
        }
        case appStateActions.SET_CURRENT_ACCOUNT_ID:{
            state.currentAccountId = action.payload;
            return { ...state };
        }
        case appStateActions.SET_CURRENT_LEGALENTITY_ID:{
            state.currentLegalEntityId = action.payload;
            return { ...state };
        }
        default:
            return state;
    }
}