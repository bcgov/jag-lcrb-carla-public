import { Action } from '@ngrx/store';
import * as ApplicationActions from '../actions/current-application.action';
import {  CurrentApplicationState } from '../models/app-state';

// Section 1
const initialState: CurrentApplicationState = { currentApplication: null };

// Section 2
export function reducer(state: CurrentApplicationState = initialState, action: ApplicationActions.Actions): CurrentApplicationState {

    // Section 3
    switch (action.type) {
        case ApplicationActions.CURRENT_APPLICATION:
            return { ...state };
        case ApplicationActions.SET_CURRENT_APPLICATION:
            return { currentApplication: action.payload };
        default:
            return state;
    }
}
