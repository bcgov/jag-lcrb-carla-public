import { Action } from '@ngrx/store';
import * as ApplicationActions from '../actions/applications.action';
import { AdoxioApplication } from '../../models/adoxio-application.model';
import { ApplicationsState } from '../models/app-state';

// Section 1
const initialState: ApplicationsState = { applications: [] };

// Section 2
export function reducer(state: ApplicationsState = initialState, action: ApplicationActions.Actions): ApplicationsState {

    // Section 3
    switch (action.type) {
        case ApplicationActions.APPLICATION:
            return { ...state };
        case ApplicationActions.SET_APPLICATION:
            return { applications: action.payload };
        default:
            return state;
    }
}
