import { Action, createReducer, on } from "@ngrx/store";
import * as StoreActions from './store.actions';
import { AppState } from "./store.state";

const initialState: AppState = {
    alerts: []
}

const _alertReducer = createReducer(
    initialState,
    on(StoreActions.addAlert, 
        (state, action) => ({ ...state, alerts: [...state.alerts, action.alert]  })),
);

export function alertReducer(state: AppState | undefined, action: Action) {
    return _alertReducer(state, action)
}