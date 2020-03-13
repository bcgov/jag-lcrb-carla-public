import * as OngoingLicenseeApplicationIdActions from '../actions/ongoing-licensee-application-id.action';
import { OnGoingLicenseeChangesApplicationIdState } from '../models/app-state';


// Section 1
const initialState: OnGoingLicenseeChangesApplicationIdState = { onGoingLicenseeChangesApplicationId: null };

// Section 2
export function reducer(state: OnGoingLicenseeChangesApplicationIdState = initialState, action: OngoingLicenseeApplicationIdActions.Actions): OnGoingLicenseeChangesApplicationIdState {

    // Section 3
    switch (action.type) {
        case OngoingLicenseeApplicationIdActions.ONGOING_LICENSEE_APPLICATION_ID:
            return { ...state };
        case OngoingLicenseeApplicationIdActions.SET_ONGOING_LICENSEE_APPLICATION_ID:
            return { onGoingLicenseeChangesApplicationId: action.payload };
        default:
            return state;
    }
}
