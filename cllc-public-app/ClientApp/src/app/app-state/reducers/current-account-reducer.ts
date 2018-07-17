import { Action } from '@ngrx/store';
import * as CurrentAccountActions from '../actions/current-account.action';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { CurrentAccountState } from '../models/app-state';

// Section 1
const initialState: CurrentAccountState = { currentAccount: null };

// Section 2
export function reducer(state: CurrentAccountState = initialState, action: CurrentAccountActions.Actions): CurrentAccountState {

    // Section 3
    switch (action.type) {
        case CurrentAccountActions.CURRENT_ACCOUNT:
            return { ...state };
        case CurrentAccountActions.SET_CURRENT_ACCOUNT:
            return { currentAccount: action.payload };
        default:
            return state;
    }
}
