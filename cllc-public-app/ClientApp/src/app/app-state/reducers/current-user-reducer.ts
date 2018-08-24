import { Action } from '@ngrx/store';
import * as CurrentUserActions from '../actions/current-user.action';
import { User } from '../../models/user.model';
import { CurrentUserState } from '../models/app-state';

// Section 1
const initialState: CurrentUserState = { currentUser: null };

// Section 2
export function reducer(state: CurrentUserState = initialState, action: CurrentUserActions.Actions): CurrentUserState {

    // Section 3
    switch (action.type) {
        case CurrentUserActions.CURRENT_USER:
            return { ...state };
        case CurrentUserActions.SET_CURRENT_USER:
            return { currentUser: action.payload };
        default:
            return state;
    }
}
