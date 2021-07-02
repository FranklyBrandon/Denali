import { createReducer, on } from "@ngrx/store";
import { addEntryAlert } from "../actions/alert.actions";
import { initialAlertState } from "../state/state.alert.model";

export const alertReducer = createReducer(
    initialAlertState,
    on(addEntryAlert, (state, action) => ({
        ...state,
        entryAlerts: [...state.entryAlerts, action.payload]
    }))
);