import * as FileUploadsActions from '../actions/file-uploads.action';
import { FileUploadsState } from '../models/app-state';

// Section 1
const initialState: FileUploadsState = { fileUploads: [] };

// Section 2
export function reducer(state: FileUploadsState = initialState, action: FileUploadsActions.Actions): FileUploadsState {

    // Section 3
    switch (action.type) {
        case FileUploadsActions.CLEAR_FILE_UPLOADS:
            return { ...state, fileUploads: state.fileUploads.filter(f => f.id !== action.payload) };
        case FileUploadsActions.SET_FILE_UPLOADS:
            return { ...state, fileUploads: [ ...state.fileUploads.filter(f => f.id !== action.payload.id), action.payload ] };
        default:
            return state;
    }
}
