import { createReducer, on } from "@ngrx/store";
import * as connectedActions from "../actions/connected.actions";
import { setConnectedStatus } from "../actions/connected.actions";

export const connectedReducer = createReducer(
    false,
    on(setConnectedStatus, (state, action) => (
        state = action.payload
    ))
);
